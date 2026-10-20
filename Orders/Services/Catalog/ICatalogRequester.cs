using Orders.Dtos;

namespace Orders.Services.Catalog
{
    public interface ICatalogRequester
    {
        public Task<List<CatalogResponse>> CheckGoodsRelevance(IEnumerable<int> goodsIds);
    }
}
