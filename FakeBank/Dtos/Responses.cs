using FakeBank.Models;

namespace FakeBank.Dtos
{
    public record PaymentResponse(
        Guid Id,
        PaymentStatus Status,
        decimal Amount,
        string Currency,
        Guid OrderId,
        string? Description,
        string ConfirmationUrl,
        CancellationReason? CancellationReason,
        decimal RefundedAmount,
        DateTime CreatedAt,
        DateTime? ProcessedAt);

    public record RefundResponse(
        Guid Id,
        Guid PaymentId,
        RefundStatus Status,
        decimal Amount,
        DateTime CreatedAt,
        DateTime? ProcessedAt);

    public record WebhookNotification(
        Guid Id,
        string Event,
        DateTime CreatedAt,
        object Object);
}
