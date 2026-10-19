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

        public async Task<OrderCreatedResponse> CreateOrder(int ownerId, List<OrderGoodDto> goods)
        {
            var status = await _ordersRepository.GetOrderStatus(OrderStatusEnum.DelayedPayment);

            var order = await _ordersRepository.CreateOrder(ownerId, status);

            await _ordersRepository.CreatePositions(goods, order.Id);

            return new OrderCreatedResponse {
                OrderId = order.Id,
                CreatedAt = order.CreatedAt, 
                Status = OrderStatusEnum.DelayedPayment
            };
        }
    }
}
