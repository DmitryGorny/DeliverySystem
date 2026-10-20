using System.Net;
using Microsoft.AspNetCore.WebUtilities;
using Orders.Common.Exceptions;
using Orders.Dtos;
using Orders.Repositories;

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
    }
    class GoodsDontExistResponse
    {
        public string Message { get; set; }
        public List<int> Data { get; set; }
    }
}
