using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.Models
{
    [Table("reservations")]
    public class Reservations
    {
        public int Id { get; set; }
        public int GoodId { get; set; }
        public Goods Good { get; set; }
        public Guid OrderId { get; set; }
        public int ReservationNumber { get; set; }
    }
}
