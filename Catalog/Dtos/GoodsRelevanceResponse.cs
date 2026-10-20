using Catalog.Common.Exceptions;

namespace Catalog.Dtos
{
    public class GoodsRelevanceResponse
    {
        public int GoodId { get; set; }
        public int AvailableQuantity { get; set; }
        public decimal ReleventPrice { get; set; }
    }
}
