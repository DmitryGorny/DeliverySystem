namespace Orders.Dtos
{
    public record PaymentSystemResponse
    {
        public Guid PaymentId { get; set; }
        public string ConfirmationUrl { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
