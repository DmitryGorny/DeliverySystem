using Orders.Common.Constants;
using Orders.Dtos;
using Orders.Repositories;
using Orders.Services.PaymentStrategy;
using Orders.Services.PaymentSystem;

namespace Orders.Services.PaymentStrategies
{
    public class CreateAndPayStrategy : IPaymentStrategy
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IPaymentRequester _paymentRequester;

        public CreateAndPayStrategy(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }
        public async Task<OrderCreatedResponse> CreateOrder(int ownerId, List<OrderGoodDto> goods)
        {
            var status = await _ordersRepository.GetOrderStatus(OrderStatusEnum.AwaitPayment);

            var order = await _ordersRepository.CreateOrder(ownerId, status);

            await _ordersRepository.CreatePositions(goods, order.Id);

            var cardentials = await _paymentRequester.GetPaymentCardentials(
                new GetPaymentCardentials 
                {
                    OrderId = order.Id,
                    Amount = goods.Sum(s => s.Price),
                    Currency = CurrencyEnum.RUB
                });

            return new OrderCreatedResponse 
            {
                OrderId = order.Id,
                CreatedAt = order.CreatedAt,
                Status = OrderStatusEnum.AwaitPayment,
                PaymentResponse = cardentials
            };
        }
    }
}
