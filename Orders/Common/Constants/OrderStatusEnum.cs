namespace Orders.Common.Constants
{
    public enum OrderStatusEnum
    {
        AwaitPayment = 1,
        DelayedPayment = 2,
        ToDelivery = 3,
        Delivered = 4,
        DeliveredWithoutPayment = 5,
        Done = 6,
        CancelledWithoutPayment = 7,
        CancellPaid = 8,
        CancellDelivered = 9,
        PaymentExpired = 10,
        PaymnetDeclined = 11,
        ReservationFailed = 12
    }
}
