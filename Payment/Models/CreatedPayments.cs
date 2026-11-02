using System.ComponentModel.DataAnnotations.Schema;

namespace Payment.Models
{
    [Table("created_payments")]
    public class CreatedPayments
    {
        public Guid Id { get; set; }
        public PaymentStatus Status { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public Guid OrderId { get; set; }
        public string Description { get; set; }
        public string ConfirmationUrl { get; set; }
        public string? CancellationReason { get; set; }
        public decimal RefundedAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
