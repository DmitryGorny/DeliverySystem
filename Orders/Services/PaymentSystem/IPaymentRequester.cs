using Orders.Dtos;

namespace Orders.Services.PaymentSystem
{
    public interface IPaymentRequester
    {
        Task<PaymentSystemResponse> GetPaymentCardentials(
            GetPaymentCardentials paymentParameters, 
            CancellationToken ct = default);
    }
}
