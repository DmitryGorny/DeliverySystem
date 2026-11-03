using System.ComponentModel.DataAnnotations;

namespace FakeBank.Dtos
{
    public record CreatePaymentRequest
    {
        [Range(typeof(decimal), "0.01", "100000000",
            ParseLimitsInInvariantCulture = true,
            ConvertValueInInvariantCulture = true)]
        public decimal Amount { get; init; }

        [RegularExpression("^RUB$", ErrorMessage = "Only RUB is supported")]
        public string Currency { get; init; } = "RUB";

        [Required]
        public Guid? OrderId { get; init; }

        [MaxLength(128)]
        public string? Description { get; init; }
    }

    public record CreateRefundRequest
    {
        [Required]
        public Guid? PaymentId { get; init; }

        [Range(typeof(decimal), "0.01", "100000000")]
        public decimal Amount { get; init; }
    }
}
