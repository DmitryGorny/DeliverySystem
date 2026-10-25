using Orders.Dtos;
using Orders.Models.enums;

namespace Orders.Services.Order
{
    public interface IOrdersService
    {
        public Task<OrderCreatedResponse> Create(int ownerId, List<OrderGoodDto> goods, OrderType orderType, CancellationToken ct = default);
        public Task Reserve(Guid orderId, CancellationToken ct = default);
    }
}
