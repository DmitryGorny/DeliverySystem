using Microsoft.EntityFrameworkCore;
using Orders.Common.Constants;
using Orders.DB;
using Orders.Dtos;
using Orders.Models;
using Orders.Models.enums;

namespace Orders.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly AppDbContext _db;

        public OrdersRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task ChangeStatus(
            Guid orderId, 
            OrderStatusEnum status, 
            CancellationToken ct = default)
        {
            var orderStatus = await _db.OrderStatuses.FirstAsync(os => os.Status.Equals(status.ToString()), ct);

            await _db.Orders
                .Where(o => o.Id == orderId)
                .ExecuteUpdateAsync(setter => setter.SetProperty(o => o.OrderStatusId, orderStatus.Id), ct);
        }

        public async Task<Models.Orders> CreateOrder(
            int ownerId, 
            OrderStatus orderStatus, 
            CancellationToken ct = default)
        {
            var order = new Models.Orders
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                OrderStatus = orderStatus,
                OrderStatusId = orderStatus.Id,
                UserId = ownerId
            };
            await _db.Orders.AddAsync(order, ct);
            await _db.SaveChangesAsync(ct);

            return order;
        }

        public async Task CreatePositions(
            List<OrderGoodDto> goodsId, 
            Guid OrderId,
            CancellationToken ct = default)
        {
            var w = goodsId.Select(async g =>
            {
                await _db.Positions.AddAsync(new Positions { 
                    GoodId = g.GoodId,
                    OrderId = OrderId, 
                    Price = g.Price,
                    Quantity = g.Quantity
                }, ct);
            });
            await Task.WhenAll(w);

            await _db.SaveChangesAsync(ct);
        }

        public Task<Models.Orders> Get(int ownerId, CancellationToken ct = default)
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

        public async Task<OrderStatus> GetOrderStatus(
            OrderStatusEnum status, 
            CancellationToken ct = default)
        {
            return await _db.OrderStatuses
                .Where(os => os.Status.Equals(status.ToString()))
                .FirstAsync(ct);
        }

        public async Task<Models.Orders?> GetByOrderId(Guid orderId, CancellationToken ct = default)
        {
            return await _db.Orders
                .Include(o => o.Positions)
                .FirstAsync(o => o.Id == orderId, ct);
        }

        public async Task CreateOrderFailure(
            Models.Orders order, 
            FailureType type, 
            List<int> positionIds, 
            CancellationToken ct = default)
        {
            var ordersFailure = new OrdersFailure
            {
                OrderId = order.Id,
                FailureType = type,
                RefundStatus = RefundStatuses.Pending
            };

            await _db.OrdersFailures.AddAsync(ordersFailure, ct);
            await _db.SaveChangesAsync(ct);

            var tasks = order.Positions
                .Where(p => positionIds.Contains(p.Id))
                .Select(async p => {
                   await _db.FailurePositions.AddAsync(new FailurePositions
                   {
                        FailureId = ordersFailure.Id,
                        PositionId = p.Id
                   }, ct);
                });

            await Task.WhenAll(tasks);
            await _db.SaveChangesAsync(ct);
        }
    }
}
