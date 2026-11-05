using Orders.Common.Constants;

namespace Orders.Dtos
{
    public class RequestPaymentCardentialsBody
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
    }
}
