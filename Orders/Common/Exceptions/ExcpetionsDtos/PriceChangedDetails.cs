namespace Orders.Common.Exceptions.ExcpetionsDtos
{
    public class PriceChangedDetails
    {
        public int GoodId { get; set; }
        public decimal RequestedPrice { get; set; }
        public decimal ReleventPrice { get; set; }
    }
}
