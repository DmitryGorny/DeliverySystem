namespace Orders.Dtos
{
    public record OrderGoodDto()
    {
        public int GoodId { get; set; }
        public int Quntity { get; set; }
        public decimal Price { get; set; }
    }
}
