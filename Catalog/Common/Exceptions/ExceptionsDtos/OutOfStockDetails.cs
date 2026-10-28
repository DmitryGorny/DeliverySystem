namespace Catalog.Common.Exceptions.ExceptionsDtos
{
    public class OutOfStockDetails : BaseDetails
    {
        public int Requested { get; set; }
        public int? Available { get; set; }
    }
}
