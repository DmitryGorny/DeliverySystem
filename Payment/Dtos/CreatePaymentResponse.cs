namespace Payment.Dtos
{
    public class CreatePaymentResponse
    {
        public Guid PaymentId { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
