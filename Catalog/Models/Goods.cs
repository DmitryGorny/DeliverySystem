namespace Catalog.Models
{
    public class Goods
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public List<Reservations> Reservations { get; set; }
        public int Reserved { get; set; }
    }
}
