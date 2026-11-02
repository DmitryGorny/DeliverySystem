using Payment.Dtos;
using Payment.Models;
using Payment.Services.BankRequester.Bank;
using Payment.Services.CreationExceptions.Fabric;

namespace Payment.Services.Bank
{
    public class BankRequester : IBankRequester
    {
        private readonly HttpClient _http;
        private readonly ServiceProvider _serviceProvider;
        public BankRequester(HttpClient http, ServiceProvider provider)
        {
            _http = http;
            _serviceProvider = provider;
        }

        public async Task<CreatedPayments> CreatePayment(CreatePaymentBody body, CancellationToken ct = default)
        {
            using var response = await _http.PostAsJsonAsync("payments", body, ct);

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

            var parsedContent = await response.Content.ReadFromJsonAsync<CreatedPayments>(ct);
            if (parsedContent == null)
                throw new Exception("Сигнатура не совпала ответа банка изменилась");

            return parsedContent;
        }
    }
}
