using Orders.Dtos;
using Orders.Models;

namespace Orders.Services.Order
{
    public interface IOrdersService
    {
        public Task<OrderCreatedResponse> Create(int ownerId, List<OrderGoodDto> goods, OrderType orderType);
    }
}
