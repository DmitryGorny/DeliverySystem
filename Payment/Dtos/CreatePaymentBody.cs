using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Payment.Dtos
{
    public class CreatePaymentBody
    {
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public Guid OrderId { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int UserId { get; set; }
    }
}
