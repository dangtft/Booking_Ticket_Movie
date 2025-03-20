using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.DTOs.Orders.Request
{
    public class OrderRequest
    {
        [Required]
        public string UserId { get; set; }  

        [Required]
        public List<Guid> SeatIds { get; set; }

        public List<Guid>? ExtraIds { get; set; } 

        public string? PaymentMethod { get; set; }
    }
}
