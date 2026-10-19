using Orders.Dtos;

namespace Orders.Services.PaymentStrategy
{
    public interface IPaymentStrategy
    {
        Task<OrderCreatedResponse> CreateOrder(int ownerId, List<OrderGoodDto> goods);
    }
}
