namespace Orders.Dtos
{
    public class GoodsReservationBody
    {
        public Guid OrderId { get; set; }
        public List<GoodToReserve> GoodsToReserve { get; set; }

    }
}
