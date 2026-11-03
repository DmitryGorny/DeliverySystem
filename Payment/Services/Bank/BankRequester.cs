using Microsoft.Extensions.Options;
using Payment.Dtos;
using Payment.Models;
using Payment.Services.BankRequester.Bank;
using Payment.Services.CreationExceptions.Fabric;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Payment.Services.Bank
{
    public class BankRequester : IBankRequester
    {
        private readonly HttpClient _http;
        private readonly IServiceProvider _serviceProvider;
        public BankRequester(HttpClient http, IServiceProvider provider)
        {
            _http = http;
            _serviceProvider = provider;
        }

        public async Task<CreatedPayments> CreatePayment(CreatePaymentBody body, CancellationToken ct = default)
        {
            using var response = await _http.PostAsJsonAsync("v1/payments", body, ct);

            if (!response.IsSuccessStatusCode)
            {
                var details = await response.Content.ReadFromJsonAsync<ProblemDetails>(ct);

                if (details == null)
                    throw new Exception("Ответ платежной системы некорректен");

                var fabric = _serviceProvider.GetKeyedService<ExceptionsFabric>(details.Code);

                if (fabric == null)
                    throw new Exception("Фабрика не найдена");

                fabric.Throw(details.Type);
            }

            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));

            var parsedContent = await response.Content.ReadFromJsonAsync<CreatedPayments>(options, ct);
            if (parsedContent == null)
                throw new Exception("Сигнатура не совпала ответа банка изменилась");

            return parsedContent;
        }
    }
}
