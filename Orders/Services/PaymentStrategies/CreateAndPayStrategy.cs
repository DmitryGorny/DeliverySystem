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

        public CreateAndPayStrategy(IOrdersRepository ordersRepository, IPaymentRequester paymentRequester)
        {
            _ordersRepository = ordersRepository;
            _paymentRequester = paymentRequester;
        }
        public async Task<OrderCreatedResponse> CreateOrder(
            int ownerId, 
            List<OrderGoodDto> goods, 
            CancellationToken ct = default)
        {
            var status = await _ordersRepository.GetOrderStatus(OrderStatusEnum.AwaitPayment, ct);

            var order = await _ordersRepository.CreateOrder(ownerId, status, ct);

            await _ordersRepository.CreatePositions(goods, order.Id, ct);

            var cardentials = await _paymentRequester.GetPaymentCardentials(
                new RequestPaymentCardentialsBody
                {
                    OrderId = order.Id,
                    Amount = goods.Sum(s => s.Price * s.Quantity),
                    Currency = CurrencyEnum.RUB.ToString(),
                    Description = $"{order.Id} заказ",
                    UserId = ownerId
                }, ct);

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
