using Orders.Common.Constants;
using Orders.Dtos;
using Orders.Models;

namespace Orders.Repositories
{
    public interface IOrdersRepository
    {
        public Task<Models.Orders> CreateOrder(int ownerId, OrderStatus orderStatus);
        public Task CreatePositions(List<OrderGoodDto> goodsId, Guid OrderId);
        public Task<Models.Orders> ChangeStatus(int orderId, OrderStatus status);
        public Task<OrderStatus> GetOrderStatus(OrderStatusEnum status);
        public Task<Models.Orders> Get(int ownerId);
    }
}
