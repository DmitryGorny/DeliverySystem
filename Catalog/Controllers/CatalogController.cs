using Catalog.Common.Exceptions;
using Catalog.Common.Exceptions.ExceptionsDtos;
using Catalog.Dtos;
using Catalog.Services.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet]
        [Route("CheckGoodsRelevance")]
        public async Task<ActionResult<GoodsRelevanceResponse>> GetRelevantGoods(
            [FromQuery] List<int> goodIds,
            CancellationToken ct = default)
        {
            try
            {
                return Ok(
                    await _catalogService.CheckGoodsRelevance(goodIds, ct));
            }
            catch (GoodsDontExistException ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,
                    Data = ex.GoodsIds
                });
            }
        }

        [HttpPost]
        [Route("ReserveGoods")]
        public async Task<ActionResult<GoodsRelevanceResponse>> ReserveGoods(
            [FromBody] CreateReservationBody body,
            CancellationToken ct = default)
        {
            try
            {
                await _catalogService.ReserveGoods(body.GoodsToReserve, body.OrderId, ct);
                return Created();
            }
            catch (GoodsDontExistException ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,
                    Data = ex.GoodsIds
                });
            }
            catch (Exception ex) when (
                ex is ReservationException<PriceChangedDetails> ||
                ex is ReservationException<OutOfStockDetails>)
            {
                return Conflict(ex);
            }
        }

    }
}
