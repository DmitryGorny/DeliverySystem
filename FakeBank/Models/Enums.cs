namespace FakeBank.Models
{
    public enum PaymentStatus
    {
        Pending,
        Succeeded,
        Canceled
    }

    public enum RefundStatus
    {
        Pending,
        Succeeded
    }

    public enum CancellationReason
    {
        InsufficientFunds,
        CardExpired,
        FraudSuspected,
        CanceledByPayer
    }
}
