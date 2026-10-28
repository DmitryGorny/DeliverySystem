using Orders.Common.Constants;
using Orders.Dtos;
using Orders.Models;
using Orders.Models.enums;

namespace Orders.Repositories
{
    public interface IOrdersRepository
    {
        public Task<Models.Orders> CreateOrder(int ownerId, OrderStatus orderStatus, CancellationToken ct = default);
        public Task CreatePositions(List<OrderGoodDto> goodsId, Guid OrderId, CancellationToken ct = default);
        public Task ChangeStatus(Guid orderId, OrderStatusEnum status, CancellationToken ct = default);
        public Task<OrderStatus> GetOrderStatus(OrderStatusEnum status, CancellationToken ct = default);
        public Task<int> SetPaymentExpired(CancellationToken ct = default);
        public Task<Models.Orders> Get(int ownerId, CancellationToken ct = default);
        public Task<Models.Orders?> GetByOrderId(Guid orderId, CancellationToken ct = default);
        public Task CreateOrderFailure(Models.Orders order, FailureType type, List<int> positionIds, CancellationToken ct = default); 
    }
}
