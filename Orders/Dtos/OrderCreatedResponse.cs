using Orders.Common.Constants;

namespace Orders.Dtos
{
    public class OrderCreatedResponse
    {
        public Guid OrderId { get; set; }
        public OrderStatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public PaymentSystemResponse? PaymentResponse { get; set; }
    }
}
