namespace Orders.Models
{
    public class FailurePositions
    {
        public int Id { get; set; }
        public int PositionId { get; set; }
        public int FailureId { get; set; }
        public Positions Position { get; set; }
        public OrdersFailure OrdersFailure { get; set; }
    }
}
