using Orders.Dtos;

namespace Orders.Services.PaymentSystem
{
    public interface IPaymentRequester
    {
        Task<PaymentSystemResponse> GetPaymentCardentials(
            RequestPaymentCardentialsBody paymentParameters, 
            CancellationToken ct = default);
    }
}
