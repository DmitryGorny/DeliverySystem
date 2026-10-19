namespace Orders.Common.Constants
{
    public enum OrderStatusEnum
    {
        AwaitPayment,
        DelayedPayment,
        ToDelivery,
        Delivered,
        DeliveredWithoutPayment,
        Done,
        CancelledWithoutPayment,
        CancellPaid,
        CancellDelivered,
        PaymentExpired,
        PaymnetDeclined,
        ReservationFailed
    }
}
