namespace Orders.Models
{
    public class Positions
    {
        public int Id { get; set; }
        public int GoodId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
    }
}
