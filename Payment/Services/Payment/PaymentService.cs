using Payment.Dtos;
using Payment.Repositories;
using Payment.Services.BankRequester.Bank;

namespace Payment.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IBankRequester _bankRequester;
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IBankRequester bankRequester, IPaymentRepository paymentRepository)
        {
            _bankRequester = bankRequester;
            _paymentRepository = paymentRepository;
        }

        public async Task<CreatePaymentResponse> CreatePayment(CreatePaymentBody body, CancellationToken ct = default)
        {
            var createdPayment = await _bankRequester.CreatePayment(body, ct);

            createdPayment.UserId = body.UserId;

            await _paymentRepository.CreatePayment(createdPayment);

            return new CreatePaymentResponse
            {
                PaymentId = createdPayment.Id,
                ConfirmationUrl = createdPayment.ConfirmationUrl,
                CreatedAt = createdPayment.CreatedAt,
                Status = createdPayment.Status.ToString()
            };
        }
    }
}
