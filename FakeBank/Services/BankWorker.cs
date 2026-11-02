using System.Net.Http.Headers;
using FakeBank.Options;
using Microsoft.Extensions.Options;

namespace FakeBank.Services
{
    // Фоновая работа банка:
    //  - в режиме Auto принимает решения по платежам;
    //  - выполняет возвраты;
    //  - доставляет webhook-и с повторами.
    public class BankWorker(
        BankService bank,
        BankStore store,
        WebhookSigner signer,
        IHttpClientFactory httpClientFactory,
        IOptions<MerchantOptions> merchant,
        TimeProvider time,
        ILogger<BankWorker> logger) : BackgroundService
    {
        public const string WebhookClientName = "webhooks";
        private const int MaxAttempts = 20;

        // Задержки перед повторами: 5 с, 15 с, 30 с, 1 мин, 2 мин, далее 5 мин
        private static readonly TimeSpan[] Backoff =
        [
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(15),
            TimeSpan.FromSeconds(30),
            TimeSpan.FromMinutes(1),
            TimeSpan.FromMinutes(2),
            TimeSpan.FromMinutes(5)
        ];

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Fake bank started. Webhooks go to {WebhookUrl}", merchant.Value.WebhookUrl);

            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(500), time);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    bank.DecideDuePayments();
                    bank.CompleteDueRefunds();
                    await DeliverWebhooksAsync(stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    logger.LogError(ex, "Bank worker iteration failed");
                }
            }
        }

        private async Task DeliverWebhooksAsync(CancellationToken ct)
        {
            var due = store.Locked(() => store.WebhooksDue(time.GetUtcNow().UtcDateTime));
            if (due.Count == 0) return;

            var client = httpClientFactory.CreateClient(WebhookClientName);

            foreach (var delivery in due)
            {
                var delivered = false;
                string? error = null;

                try
                {
                    using var content = new ByteArrayContent(delivery.Body);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" };

                    using var request = new HttpRequestMessage(HttpMethod.Post, merchant.Value.WebhookUrl) { Content = content };
                    request.Headers.Add(WebhookSigner.HeaderName, signer.Sign(delivery.Body));

                    using var response = await client.SendAsync(request, ct);
                    delivered = response.IsSuccessStatusCode;
                    error = delivered ? null : $"HTTP {(int)response.StatusCode}";
                }
                catch (Exception ex) when (ex is HttpRequestException || (ex is TaskCanceledException && !ct.IsCancellationRequested))
                {
                    error = ex.Message;
                }

                var now = time.GetUtcNow().UtcDateTime;

                store.Locked(() =>
                {
                    if (delivered)
                    {
                        store.RemoveWebhook(delivery.Id);
                        return;
                    }

                    delivery.Attempts++;
                    if (delivery.Attempts >= MaxAttempts)
                    {
                        store.RemoveWebhook(delivery.Id);
                        logger.LogError("Webhook {Event} {Id} dropped after {Attempts} attempts", delivery.Event, delivery.Id, delivery.Attempts);
                        return;
                    }

                    var delay = Backoff[Math.Min(delivery.Attempts - 1, Backoff.Length - 1)];
                    delivery.NextAttemptAt = now + delay;
                    logger.LogWarning("Webhook {Event} {Id} not delivered ({Error}), attempt {Attempt}, retry in {Delay}",
                        delivery.Event, delivery.Id, error, delivery.Attempts, delay);
                });

                if (delivered)
                {
                    logger.LogInformation("Webhook {Event} {Id} delivered", delivery.Event, delivery.Id);
                }
            }
        }
    }
}
