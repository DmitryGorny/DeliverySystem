using Orders.Common.Constants;
using Orders.Dtos;
using Orders.Repositories;
using Orders.Services.PaymentStrategy;

namespace Orders.Services.PaymentStrategies
{
    public class DelayedPaymentStrategy : IPaymentStrategy
    {
        private readonly IOrdersRepository _ordersRepository;

        public DelayedPaymentStrategy(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        public async Task<OrderCreatedResponse> CreateOrder(
            int ownerId, 
            List<OrderGoodDto> goods, 
            CancellationToken ct = default)
        {
            var status = await _ordersRepository.GetOrderStatus(OrderStatusEnum.DelayedPayment, ct);

            var order = await _ordersRepository.CreateOrder(ownerId, status, ct);

            await _ordersRepository.CreatePositions(goods, order.Id, ct);

            return new OrderCreatedResponse {
                OrderId = order.Id,
                CreatedAt = order.CreatedAt, 
                Status = OrderStatusEnum.DelayedPayment
            };
        }
    }
}
