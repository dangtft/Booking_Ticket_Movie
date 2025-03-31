using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.DTOs.Orders.Request
{
    public class OrderTicketRequest
    {

        [Required]
        public Guid ShowTimeId { get; set; }

        [Required]
        public Guid SeatId { get; set; }

        [Required]
        public decimal TicketPrice { get; set; }
    }
}
