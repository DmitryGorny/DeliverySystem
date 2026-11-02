namespace FakeBank.Models
{
    public class Payment
    {
        public Guid Id { get; init; }
        public Guid OrderId { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; init; } = "RUB";
        public string? Description { get; init; }

        public PaymentStatus Status { get; set; }
        public CancellationReason? CancellationReason { get; set; }

        // Сумма всех созданных возвратов (pending + succeeded)
        public decimal RefundedAmount { get; set; }

        public DateTime CreatedAt { get; init; }
        public DateTime? ProcessedAt { get; set; }

        // Когда банк в режиме Auto примет решение
        public DateTime DecideAt { get; init; }

        // Платёж «завис»: в режиме Auto банк никогда не примет по нему решение
        public bool Hangs { get; init; }
    }

    public class Refund
    {
        public Guid Id { get; init; }
        public Guid PaymentId { get; init; }
        public decimal Amount { get; init; }

        public RefundStatus Status { get; set; }

        public DateTime CreatedAt { get; init; }
        public DateTime? ProcessedAt { get; set; }

        // Когда возврат будет выполнен
        public DateTime CompleteAt { get; init; }
    }

    // Уведомление в очереди на отправку мерчанту
    public class WebhookDelivery
    {
        public Guid Id { get; init; }
        public string Event { get; init; } = "";

        // Тело сериализуется один раз, чтобы подпись совпадала с отправленными байтами
        public byte[] Body { get; init; } = [];

        public int Attempts { get; set; }
        public DateTime NextAttemptAt { get; set; }
    }
}
