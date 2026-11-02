using Payment.Dtos;

namespace Payment.Services.Payment
{
    public interface IPaymentService
    {
        Task<CreatePaymentResponse> CreatePayment(CreatePaymentBody body, CancellationToken ct = default);
    }
}
