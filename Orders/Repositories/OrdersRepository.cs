using Microsoft.EntityFrameworkCore;
using Orders.Common.Constants;
using Orders.DB;
using Orders.Dtos;
using Orders.Models;

namespace Orders.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly AppDbContext _db;

        public OrdersRepository(AppDbContext db)
        {
            _db = db;
        }
        public Task<Models.Orders> ChangeStatus(int orderId, OrderStatus status)
        {
            throw new NotImplementedException();
        }

        public Task<Models.Orders> CreateOrder(int ownerId, OrderStatus orderStatus)
        {
            throw new NotImplementedException();
        }

        public Task CreatePositions(List<OrderGoodDto> goodsId, Guid OrderId)
        {
            throw new NotImplementedException();
        }

        public Task<Models.Orders> Get(int ownerId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> SetPaymentExpired(CancellationToken ct = default)
        {
            var status = await _db.OrderStatuses
                .Where(os => os.Status.Equals(OrderStatusEnum.PaymentExpired.ToString()))
                .FirstAsync(ct); //Не может быть пустым, т.к. статусы из констант обзяаны быть в таблице

            return await _db.Orders
                .Include(o => o.OrderStatus)
                .Where(o => o.OrderStatus.Status.Equals(OrderStatusEnum.AwaitPayment.ToString()) 
                && o.CreatedAt <= DateTime.UtcNow.AddMinutes(-15))
                .ExecuteUpdateAsync(setter => setter.SetProperty(o => o.OrderStatusId, status.Id), ct);
        }

        Task<OrderStatus> IOrdersRepository.GetOrderStatus(OrderStatusEnum status)
        {
            throw new NotImplementedException();
        }
    }
}
