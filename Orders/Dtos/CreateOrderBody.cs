using Orders.Models;
using System.ComponentModel.DataAnnotations;

namespace Orders.Dtos
{
    public class CreateOrderBody
    {
        public int ownerId { get; set; }

        [Required]
        public List<OrderGoodDto> Goods { get; set; }

        [Required]
        public OrderType OrderType { get; set; }
    }
}
