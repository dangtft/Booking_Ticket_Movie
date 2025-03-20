using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.Models.Payments
{
    public class PaymentStatus
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string StatusName { get; set; }
    }

}
