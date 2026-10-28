using Orders.Common.Constants;
using Orders.Common.Exceptions;
using Orders.Common.Exceptions.ExcpetionsDtos;
using Orders.Dtos;
using Orders.Models.enums;
using Orders.Repositories;
using Orders.Services.Catalog;
using Orders.Services.PaymentStrategy;

namespace Orders.Services.Order
{
    public class OrdersService : IOrdersService
    {
        private readonly ICatalogRequester _catalogRequester;
        private readonly IOrdersRepository _ordersRepository;
        private readonly IServiceProvider _serviceProvider;
        public OrdersService(ICatalogRequester catalogRequester, IServiceProvider provider)
        {
            _catalogRequester = catalogRequester;
            _serviceProvider = provider;
        }

        public async Task<OrderCreatedResponse> Create(
            int ownerId, 
            List<OrderGoodDto> goods, 
            OrderType orderType,
            CancellationToken ct = default)
        {
            
            var response = await _catalogRequester.CheckGoodsRelevance(goods.Select(g => g.GoodId), ct);

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

            return await strategy.CreateOrder(ownerId, goods, ct);
        }

        public async Task Reserve(Guid orderId, CancellationToken ct = default)
        {
            var order = await _ordersRepository.GetByOrderId(orderId, ct);

            if (order == null)
                return; //TODO: подумать что тут делать

            try
            {
                await _catalogRequester.ReserveGoods(new GoodsReservationBody
                {
                    OrderId = orderId,
                    GoodsToReserve = order.Positions
                    .Select(p =>
                        new GoodToReserve
                        {
                            GoodId = p.GoodId,
                            Quantity = p.Quantity
                        }).ToList()
                    }, ct);
                return;
            } catch (GoodsDontExistException ex)
            {
                await _ordersRepository.CreateOrderFailure(
                    order, 
                    FailureType.GoodsDontExist, 
                    ex.Goods,
                    ct);
            }
            catch (PriceChangedExcpetion ex)
            {
                await _ordersRepository.CreateOrderFailure(
                    order,
                    FailureType.PriceChanged,
                    ex.Details.Select(d => d.GoodId).ToList(),
                    ct);
            }
            catch (GoodOutOfStockException ex)
            {
                await _ordersRepository.CreateOrderFailure(
                    order,
                    FailureType.OutOfStock,
                    ex.Goods.Select(d => d.GoodId).ToList(),
                    ct);
            }

            await _ordersRepository.ChangeStatus(orderId, OrderStatusEnum.ReservationFailed, ct);
        }

        private IPaymentStrategy GetStrategy(OrderType orderType)
        {
            return _serviceProvider.GetKeyedService<IPaymentStrategy>(orderType)!;
        }
    }
}
