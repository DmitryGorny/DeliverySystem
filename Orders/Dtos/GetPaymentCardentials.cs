using Orders.Common.Constants;

namespace Orders.Dtos
{
    public class GetPaymentCardentials
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public CurrencyEnum Currency { get; set; }
    }
}
