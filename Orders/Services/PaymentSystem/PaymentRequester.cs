using Orders.Common.Exceptions.PaymentExceptions;
using Orders.Dtos;

namespace Orders.Services.PaymentSystem
{
    public class PaymentRequester : IPaymentRequester
    {
        private readonly HttpClient _http;

        public PaymentRequester(HttpClient http)
        {
            _http = http;
        }
        public async Task<PaymentSystemResponse> GetPaymentCardentials(RequestPaymentCardentialsBody paymentParameters, CancellationToken ct = default)
        {
            using var response = await _http.PostAsJsonAsync("CreatePayment", paymentParameters, ct);

            if (!response.IsSuccessStatusCode)
            {
                var exception = await response.Content.ReadFromJsonAsync<PaymentSystemException>();

                if (exception == null)
                    throw new PaymentSystemException("Сервис ответил ошибкой", 500);

                throw exception;
            }

            //Не null, т.к. структура ответа в случае создания платежа всегда поступит
            return (await response.Content.ReadFromJsonAsync<PaymentSystemResponse>())!;
        }
    }
}
