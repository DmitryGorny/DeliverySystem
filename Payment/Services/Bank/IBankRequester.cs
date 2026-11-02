using Payment.Dtos;
using Payment.Models;

namespace Payment.Services.BankRequester.Bank
{
    public interface IBankRequester
    {
        Task<CreatedPayments> CreatePayment(CreatePaymentBody body, CancellationToken ct = default);
    }
}
