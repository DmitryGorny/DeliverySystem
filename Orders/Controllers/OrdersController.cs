using Microsoft.AspNetCore.Mvc;
using Orders.Common.Exceptions;
using Orders.Dtos;
using Orders.Services.Order;

namespace Orders.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;

        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        [HttpPost]
        [Route("CreateOrder")]
        public async Task<ActionResult<OrderCreatedResponse>> CreateOrder(
            [FromBody] CreateOrderBody body,
            CancellationToken ct = default)
            //Временно ownerId в Body, потом будет браться из клеймов
        {
            try
            {
                return await _ordersService.Create(body.ownerId, body.Goods, body.OrderType, ct);
            } catch (GoodOutOfStockException ex)
            {
                return Conflict(new {
                    Message = ex.Message, 
                    Data = ex.Goods 
                });
            } catch (GoodsDontExistException ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,
                    Data = ex.Goods
                });
            }
        }
    }
}
