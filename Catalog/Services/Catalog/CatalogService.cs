using Catalog.Common.Exceptions;
using Catalog.Dtos;
using Catalog.Repositories;

namespace Catalog.Services.Catalog
{
    public class CatalogService : ICatalogService
    {
        private readonly ICatalogRepository _catalogRepository;

        public CatalogService(ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
        }

        public async Task<List<GoodsRelevanceResponse>> CheckGoodsRelevance(List<int> goodsIds)
        {
            var goods = await _catalogRepository.GetGoodsByIds(goodsIds);
                
            if (goods.Count < goodsIds.Count)
            {
                var foundGoodsId = goods.Select(g => g.Id).ToHashSet();
                throw new GoodsDontExistException(
                    "Переданных id товаров не существуют",
                    goodsIds.Where(id => !foundGoodsId.Contains(id)).ToList());
            }

            return goods.Select(g => new GoodsRelevanceResponse 
            {
                GoodId = g.Id,
                AvailableQuantity = g.Quantity - g.Reserved,
                ReleventPrice = g.Price
            }).ToList();
        }
    }
}
