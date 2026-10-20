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
        public Task CreateReservation(List<ReservationGood> goods, Guid OrderId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Goods>> GetGoodsByIds(List<int> goodsIds)
        {
            return await _db.Goods
                .Where(g => goodsIds.Contains(g.Id))
                .ToListAsync();
        }
    }
}
