using Catalog.DB;
using Catalog.Dtos;
using Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly AppDbContext _db;

        public CatalogRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task CreateReservation(List<GoodToReserve> goods, Guid orderId, CancellationToken ct = default)
        {
            var tasks = goods.Select(async g =>
            {
                var resrvation = new Reservations
                {
                    OrderId = orderId,
                    GoodId = g.GoodId,
                    ReservationNumber = g.Quantity,
                };

                return await _db.Reservations.AddAsync(resrvation, ct);
            });

            await Task.WhenAll(tasks);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<List<Goods>> GetGoodsByIds(List<int> goodsIds, CancellationToken ct = default)
        {
            return await _db.Goods
                .Where(g => goodsIds.Contains(g.Id))
                .ToListAsync(ct);
        }
    }
}
