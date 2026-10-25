using Orders.Models.enums;

namespace Orders.Models
{
    public class OrdersFailure
    {
        public int Id { get; set; }
        public FailureType FailureType { get; set; }
        public Guid OrderId { get; set; }
        public Orders Order { get; set; }
        public string Reason { get; set; }
        public RefundStatuses RefundStatus { get; set; }
    }
}
