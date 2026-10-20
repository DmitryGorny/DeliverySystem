using Orders.Common.Constants;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orders.Models
{
    [Table("positions")]
    public class Positions
    {
        public int Id { get; set; }
        public int GoodId { get; set; }
        public Guid OrderId { get; set; }
        public Orders Order { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
