using Catalog.Dtos;
using Catalog.Models;

namespace Catalog.Repositories
{
    public interface ICatalogRepository
    {
        Task<List<Goods>> GetGoodsByIds(List<int> goodsIds, CancellationToken ct = default);
        Task CreateReservation(List<GoodToReserve> goods, Guid orderId, CancellationToken ct = default);
    }
}
