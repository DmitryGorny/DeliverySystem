using Catalog.Dtos;

namespace Catalog.Services.Catalog
{
    public interface ICatalogService
    {
        Task<List<GoodsRelevanceResponse>> CheckGoodsRelevance(List<int> goodsIds); 
    }
}
