using Orders.Dtos;

namespace Orders.Services.PaymentSystem
{
    public class PaymentRequester : IPaymentRequester
    {
        public Task<PaymentSystemResponse> GetPaymentCardentials(GetPaymentCardentials paymentParameters)
        {
            return Task.FromResult(new PaymentSystemResponse { 
                CreatedAt = DateTime.UtcNow, 
                PaymentId = Guid.NewGuid(),
                Status = "pending",
                Url = "OCHKO"
            });
        }
    }
}
