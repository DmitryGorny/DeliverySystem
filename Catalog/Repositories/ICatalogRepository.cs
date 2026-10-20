using Catalog.Dtos;
using Catalog.Models;

namespace Catalog.Repositories
{
    public interface ICatalogRepository
    {
        Task<Goods> GetGoodsByIds(List<int> goodsIds);
        Task CreateReservation(List<ReservationGood> goods, Guid OrderId);
    }
}
