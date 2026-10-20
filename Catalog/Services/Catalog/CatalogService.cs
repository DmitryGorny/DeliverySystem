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
                throw new GoodsDontExistException(
                    "Переданных id товаров не существуют",
                    goods.Where(g => !goodsIds.Contains(g.Id)).Select(g => g.Id).ToList());
            }

            return goods.Select(g => new GoodsRelevanceResponse 
            {
                GoodId = g.Id,
                AvailableQuantity = g.Quantity,
                ReleventPrice = g.Price
            }).ToList();
        }
    }
}
