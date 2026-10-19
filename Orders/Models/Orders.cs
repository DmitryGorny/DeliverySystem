
namespace Orders.Models
{
    public class Orders
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }

        public List<int> PositionsIds { get; set; }
        public List<Positions> Positions { get; set; }

        public int OrderStatusId { get; set; }
        public OrderStatus OrderStatus { get; set; } 

        public DateTime CreatedAt { get; set; }
    }
}
