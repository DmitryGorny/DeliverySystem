using Orders.Common.Exceptions;
using Orders.Common.Exceptions.ExcpetionsDtos;
using Orders.Dtos;
using Orders.Models;
using Orders.Repositories;
using Orders.Services.Catalog;
using Orders.Services.PaymentStrategy;

namespace Orders.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly ICatalogRequester _catalogRequester;
        private readonly IOrdersRepository _ordersRepository;
        private readonly IServiceProvider _serviceProvider;
        public OrderService(ICatalogRequester catalogRequester, IServiceProvider provider)
        {
            _catalogRequester = catalogRequester;
            _serviceProvider = provider;
        }
        public async Task<OrderCreatedResponse> Create(int ownerId, List<OrderGoodDto> goods, OrderType orderType)
        {
            
            var response = await _catalogRequester.GetGoodsQuantity(goods.Select(g => g.GoodId));

            var joinedGoods = response.RightJoin(goods, r => r.GoodId, g => g.GoodId, (r, g) => new
            {
                GoodId = g.GoodId,
                RequestedQuantity = g.Quantity,
                AvailableQuantity = r.AvailableQuantity,
                RequestedPrice = g.Price,
                ReleventPrice = r.ReleventPrice
            });

            var unavailableQuantity = joinedGoods.Where(rg => rg.AvailableQuantity - rg.RequestedQuantity < 0);

            if (unavailableQuantity.Any())
            {
                throw new GoodOutOfStockException("Выбранный товар закончился", unavailableQuantity
                    .Select(rg => new OutOfStockDetails
                    {
                        GoodId = rg.GoodId,
                        Available = rg.AvailableQuantity,
                        Requested = rg.RequestedQuantity
                    }).ToList());
            }

            var changedPrice = joinedGoods.Where(rg => rg.RequestedPrice != rg.ReleventPrice);

            if (changedPrice.Any())
            {
                throw new PriceChangedExcpetion("Цена на товар изменилась", changedPrice
                    .Select(rg => new PriceChangedDetails
                    {
                        GoodId = rg.GoodId,
                        ReleventPrice = rg.ReleventPrice,
                        RequestedPrice = rg.RequestedPrice
                    }).ToList());
            }

            var strategy = GetStrategy(orderType);

            return await strategy.CreateOrder(ownerId, goods);
        }

        private IPaymentStrategy GetStrategy(OrderType orderType)
        {
            return _serviceProvider.GetKeyedService<IPaymentStrategy>(orderType)!;
        }
    }
}
