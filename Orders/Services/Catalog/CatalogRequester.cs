using Microsoft.AspNetCore.WebUtilities;
using Orders.Common.Exceptions;
using Orders.Common.Exceptions.ExcpetionsDtos;
using Orders.Dtos;
using Orders.Models.enums;
using Orders.Repositories;
using System.Net;
using System.Text.Json;

namespace Orders.Services.Catalog
{
    public class CatalogRequester : ICatalogRequester
    {
        private readonly HttpClient _http;
        public CatalogRequester(HttpClient http, IOrdersRepository ordersRepository)
        {
            _http = http;
        }

        public async Task<List<CatalogResponse>> CheckGoodsRelevance(
            IEnumerable<int> goodsIds, 
            CancellationToken ct = default)
        {
            string query = QueryHelpers.AddQueryString(
                "CheckGoodsRelevance",
                goodsIds.Select(id => KeyValuePair.Create("goodIds", id.ToString()))!);

            using var response = await _http.GetAsync(query, ct);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseParsed = await response.Content.ReadFromJsonAsync<GoodsDontExistResponse>(ct);

                //responsedParsed != null т.к. по контракту BadRequest по CheckGoodsRelevance возвращает GoodsDontExistResponse
                throw new GoodsDontExistException(responseParsed!.Message, responseParsed.Data);
            }

            using HttpResponseMessage message = response.EnsureSuccessStatusCode();

            //CheckGoodsRelevance возвратит результат, который парсится в CatalogResponse
            return (await message.Content.ReadFromJsonAsync<List<CatalogResponse>>(ct))!;
        }

        public async Task ReserveGoods(GoodsReservationBody body, CancellationToken ct = default)
        {            
            using var response = await _http.PostAsJsonAsync("ReserveGoods", body, ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var responseParsed = await response.Content.ReadFromJsonAsync<GoodsDontExistResponse>(ct);

                throw new GoodsDontExistException(responseParsed!.Message, responseParsed.Data);
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                var responseParsed = await response.Content.ReadFromJsonAsync<ReservationExceptionResponse>(ct);

                if (responseParsed!.Type == FailureType.OutOfStock)
                    throw new GoodOutOfStockException(responseParsed.Message, responseParsed.OutOfStock!);

                if (responseParsed!.Type == FailureType.PriceChanged)
                    throw new PriceChangedExcpetion(responseParsed.Message, responseParsed.PriceChanged!);
            }

            response.EnsureSuccessStatusCode();
        }
    }
    class GoodsDontExistResponse
    {
        public string Message { get; set; }
        public List<int> Data { get; set; }
    }

    class ReservationExceptionResponse
    {
        public string Message { get; set; }
        public FailureType Type { get; set; }
        public List<OutOfStockDetails>? OutOfStock { get; set; }
        public List<PriceChangedDetails>? PriceChanged { get; set; }
    }
}
