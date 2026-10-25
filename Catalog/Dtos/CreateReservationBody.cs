namespace Catalog.Dtos
{
    public class CreateReservationBody
    {
        public Guid OrderId { get; set; }
        public List<GoodToReserve> GoodsToReserve { get; set; }
    }
}
