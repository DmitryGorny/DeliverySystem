namespace Catalog.Common.Exceptions.ExceptionsDtos
{
    public class PriceChangedDetails : BaseDetails
    {
        public decimal RequestedPrice { get; set; }
        public decimal ReleventPrice { get; set; }
    }
}
