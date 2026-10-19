using System.ComponentModel.DataAnnotations.Schema;

namespace Orders.Models
{
    [Table("order_status")]
    public class OrderStatus
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }
}
