using Catalog.Dtos;

namespace Catalog.Services.Catalog
{
    public interface ICatalogService
    {
        Task<List<GoodsRelevanceResponse>> CheckGoodsRelevance(List<int> goodsIds, CancellationToken ct = default);
        Task ReserveGoods(List<GoodToReserve> reservationGoods, Guid orderId, CancellationToken ct = default);
    }
}
