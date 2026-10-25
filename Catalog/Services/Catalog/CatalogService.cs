using Catalog.Common.Exceptions;
using Catalog.Common.Exceptions.ExceptionsDtos;
using Catalog.Dtos;
using Catalog.Dtos.enums;
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

        public async Task<List<GoodsRelevanceResponse>> CheckGoodsRelevance(List<int> goodsIds, CancellationToken ct = default)
        {
            var goods = await _catalogRepository.GetGoodsByIds(goodsIds, ct);

            CheckGoods(goods, goodsIds);

            return goods.Select(g => new GoodsRelevanceResponse 
            {
                GoodId = g.Id,
                AvailableQuantity = g.Quantity - g.Reserved,
                ReleventPrice = g.Price
            }).ToList();
        }

        public async Task ReserveGoods(
            List<GoodToReserve> reservationGoods, 
            Guid orderId, 
            CancellationToken ct = default)
        {
            var goodsIds = reservationGoods.Select(g => g.GoodId).ToList();
            var goods = await _catalogRepository.GetGoodsByIds(goodsIds, ct);

            CheckGoods(goods, goodsIds);

            var joinedGoods = goods.Join(reservationGoods, g => g.Id, rg => rg.GoodId, (g, rg) => new
            {
                Id = g.Id,
                ReleventPrice = rg.Price,   
                CurrentPrice = g.Price,
                AvailableQuantity = g.Quantity,
                CurrentQuantity = rg.Quantity
            });

            var goodsUnavailablePrice = joinedGoods.Where(g => g.ReleventPrice - g.CurrentPrice < 0).ToList();
            if (goodsUnavailablePrice.Count > 0)
                throw new ReservationException<PriceChangedDetails>(
                    "Цена на товары в заказе не соответсвует цене на складе",
                    goodsUnavailablePrice.Select(g => new PriceChangedDetails
                    {
                        GoodId = g.Id,
                        RequestedPrice = g.CurrentPrice,
                        ReleventPrice = g.ReleventPrice
                    }).ToList(),
                    FailureType.PriceChanged
                );

            var goodsOutOfStock = joinedGoods.Where(g => g.AvailableQuantity - g.CurrentQuantity < 0).ToList();
            if (goodsOutOfStock.Count > 0)
                throw new ReservationException<OutOfStockDetails>(
                    "Товары на скалде закончились",
                    goodsOutOfStock.Select(g => new OutOfStockDetails
                    {
                        GoodId = g.Id,
                        Available = g.AvailableQuantity,
                        Requested = g.CurrentQuantity
                    }).ToList(),
                    FailureType.OutOfStock
                );

            await _catalogRepository.CreateReservation(reservationGoods, orderId, ct);
        }

        private void CheckGoods(List<Models.Goods> goods, List<int> goodsIds)
        {
            if (goods.Count < goodsIds.Count)
            {
                var foundGoodsId = goods.Select(g => g.Id).ToHashSet();
                throw new GoodsDontExistException(
                    "Переданных id товаров не существуют",
                    goodsIds.Where(id => !foundGoodsId.Contains(id)).ToList());
            }
        } 
    }
}
