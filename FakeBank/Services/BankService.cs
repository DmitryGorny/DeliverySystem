using FakeBank.Dtos;
using FakeBank.Models;
using FakeBank.Options;
using Microsoft.Extensions.Options;

namespace FakeBank.Services
{
    public enum DecideResult
    {
        Done,
        NotFound,
        NotPending
    }

    public enum RefundCreateResult
    {
        Created,
        PaymentNotFound,
        NotAllowed,
        AmountExceeded
    }

    public class BankService(BankStore store, IOptions<BankOptions> options, TimeProvider time)
    {
        private readonly BankOptions _options = options.Value;

        private DateTime Now => time.GetUtcNow().UtcDateTime;

        public PaymentResponse CreatePayment(CreatePaymentRequest request)
        {
            var now = Now;
            var delay = Random.Shared.Next(_options.MinDelaySeconds, _options.MaxDelaySeconds + 1);

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = request.OrderId!.Value,
                Amount = request.Amount,
                Currency = request.Currency,
                Description = request.Description,
                Status = PaymentStatus.Pending,
                CreatedAt = now,
                DecideAt = now.AddSeconds(delay),
                Hangs = Random.Shared.NextDouble() < _options.HangRate
            };

            store.Locked(() => store.AddPayment(payment));
            return ToResponse(payment);
        }

        public PaymentResponse? GetPayment(Guid id) =>
            store.Locked(() => store.FindPayment(id) is { } p ? ToResponse(p) : null);

        // Исход платежа: из фоновой задачи (Auto) или из /sandbox
        public DecideResult Decide(Guid paymentId, bool success, CancellationReason reason = CancellationReason.InsufficientFunds)
        {
            var now = Now;

            return store.Locked(() =>
            {
                var payment = store.FindPayment(paymentId);
                if (payment is null) return DecideResult.NotFound;
                if (payment.Status != PaymentStatus.Pending) return DecideResult.NotPending;

                payment.Status = success ? PaymentStatus.Succeeded : PaymentStatus.Canceled;
                payment.CancellationReason = success ? null : reason;
                payment.ProcessedAt = now;

                store.EnqueueWebhook(success ? "payment.succeeded" : "payment.canceled", ToResponse(payment), now);
                return DecideResult.Done;
            });
        }

        public (RefundCreateResult Result, RefundResponse? Refund) CreateRefund(CreateRefundRequest request)
        {
            var now = Now;

            return store.Locked<(RefundCreateResult, RefundResponse?)>(() =>
            {
                var payment = store.FindPayment(request.PaymentId!.Value);
                if (payment is null) return (RefundCreateResult.PaymentNotFound, null);
                if (payment.Status != PaymentStatus.Succeeded) return (RefundCreateResult.NotAllowed, null);
                if (payment.RefundedAmount + request.Amount > payment.Amount) return (RefundCreateResult.AmountExceeded, null);

                var refund = new Refund
                {
                    Id = Guid.NewGuid(),
                    PaymentId = payment.Id,
                    Amount = request.Amount,
                    Status = RefundStatus.Pending,
                    CreatedAt = now,
                    CompleteAt = now.AddSeconds(_options.RefundDelaySeconds)
                };

                payment.RefundedAmount += refund.Amount;
                store.AddRefund(refund);
                return (RefundCreateResult.Created, ToResponse(refund));
            });
        }

        public RefundResponse? GetRefund(Guid id) =>
            store.Locked(() => store.FindRefund(id) is { } r ? ToResponse(r) : null);

        // Вызывается фоновой задачей
        public int DecideDuePayments()
        {
            if (_options.Mode != BankMode.Auto) return 0;

            var due = store.Locked(() => store.PaymentsDueForDecision(Now));
            var reasons = Enum.GetValues<CancellationReason>();

            foreach (var payment in due)
            {
                var success = Random.Shared.NextDouble() >= _options.FailureRate;
                var reason = reasons[Random.Shared.Next(reasons.Length)];
                Decide(payment.Id, success, reason);
            }

            return due.Count;
        }

        public int CompleteDueRefunds()
        {
            var now = Now;

            return store.Locked(() =>
            {
                var due = store.RefundsDueForCompletion(now);
                foreach (var refund in due)
                {
                    refund.Status = RefundStatus.Succeeded;
                    refund.ProcessedAt = now;
                    store.EnqueueWebhook("refund.succeeded", ToResponse(refund), now);
                }
                return due.Count;
            });
        }

        private PaymentResponse ToResponse(Payment p) => new(
            p.Id, p.Status, p.Amount, p.Currency, p.OrderId, p.Description,
            $"{_options.PublicBaseUrl.TrimEnd('/')}/pay/{p.Id}",
            p.CancellationReason, p.RefundedAmount, p.CreatedAt, p.ProcessedAt);

        private static RefundResponse ToResponse(Refund r) => new(
            r.Id, r.PaymentId, r.Status, r.Amount, r.CreatedAt, r.ProcessedAt);
    }
}
