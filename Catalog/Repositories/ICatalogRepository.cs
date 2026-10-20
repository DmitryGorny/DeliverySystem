using Catalog.Dtos;
using Catalog.Models;

namespace Catalog.Repositories
{
    public interface ICatalogRepository
    {
        Task<List<Goods>> GetGoodsByIds(List<int> goodsIds);
        Task CreateReservation(List<ReservationGood> goods, Guid OrderId);
    }
}
