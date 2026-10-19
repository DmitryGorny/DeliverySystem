namespace Orders.Dtos
{
    public record CatalogResponse
    {
        public int GoodId { get; set; }
        public int AvailableQuantity { get; set; }
        public decimal ReleventPrice { get; set; }
    }
}
