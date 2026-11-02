using Payment.Models;

namespace Payment.Repositories
{
    public interface IPaymentRepository
    {
        Task<CreatedPayments> CreatePayment(CreatedPayments paymentCreated);
    }
}
