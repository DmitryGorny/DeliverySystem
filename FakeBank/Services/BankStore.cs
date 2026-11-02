using System.Text.Json;
using FakeBank.Dtos;
using FakeBank.Infrastructure;
using FakeBank.Models;

namespace FakeBank.Services
{
    // In-memory хранилище банка. Все операции под одним замком — для эмулятора этого достаточно.
    public class BankStore
    {
        private readonly object _sync = new();
        private readonly Dictionary<Guid, Payment> _payments = new();
        private readonly Dictionary<Guid, Refund> _refunds = new();
        private readonly Dictionary<Guid, WebhookDelivery> _webhooks = new();

        public T Locked<T>(Func<T> action)
        {
            lock (_sync) { return action(); }
        }

        public void Locked(Action action)
        {
            lock (_sync) { action(); }
        }

        // Ниже — методы, которые вызываются только внутри Locked(...)

        public void AddPayment(Payment payment) => _payments[payment.Id] = payment;
        public Payment? FindPayment(Guid id) => _payments.GetValueOrDefault(id);

        public void AddRefund(Refund refund) => _refunds[refund.Id] = refund;
        public Refund? FindRefund(Guid id) => _refunds.GetValueOrDefault(id);

        public List<Payment> PaymentsDueForDecision(DateTime now) =>
            _payments.Values.Where(p => p.Status == PaymentStatus.Pending && !p.Hangs && p.DecideAt <= now).ToList();

        public List<Refund> RefundsDueForCompletion(DateTime now) =>
            _refunds.Values.Where(r => r.Status == RefundStatus.Pending && r.CompleteAt <= now).ToList();

        public List<WebhookDelivery> WebhooksDue(DateTime now) =>
            _webhooks.Values.Where(w => w.NextAttemptAt <= now).ToList();

        public void EnqueueWebhook(string eventName, object payload, DateTime now)
        {
            var notification = new WebhookNotification(Guid.NewGuid(), eventName, now, payload);
            var delivery = new WebhookDelivery
            {
                Id = notification.Id,
                Event = eventName,
                Body = JsonSerializer.SerializeToUtf8Bytes(notification, BankJson.Options),
                NextAttemptAt = now
            };
            _webhooks[delivery.Id] = delivery;
        }

        public void RemoveWebhook(Guid id) => _webhooks.Remove(id);
    }
}
