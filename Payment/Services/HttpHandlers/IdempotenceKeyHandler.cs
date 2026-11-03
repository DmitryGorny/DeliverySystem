using Payment.Dtos;
using System.Net.Http.Headers;

namespace Payment.Services.HttpHandlers
{
    public class IdempotenceKeyHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request.Content);

            var body = await request.Content.ReadFromJsonAsync<CreatePaymentBody>();

            ArgumentNullException.ThrowIfNull(body);

            request.Headers.Add("Idempotence-Key", body.OrderId.ToString());

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
