using Catalog.Common.Exceptions;
using Catalog.Dtos;
using Catalog.Services.Catalog;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<GoodsRelevanceResponse>> GetRelevantGoods([FromQuery] List<int> goodIds)
        {
            try
            {
                return Ok(
                    await _catalogService.CheckGoodsRelevance(goodIds));
            } catch (GoodsDontExistException ex)
            {
                return BadRequest(new {
                    Message = ex.Message,
                    Data = ex.GetGoods()
                });
            }
        }
    }
}
