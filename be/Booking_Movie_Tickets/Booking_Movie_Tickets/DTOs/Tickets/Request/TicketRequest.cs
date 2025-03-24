using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.DTOs.Tickets.Request
{
    public class TicketRequest
    {

        [Required]
        public Guid ShowTimeId { get; set; }

        [Required]
        public Guid SeatId { get; set; }

        [Required]
        public Guid TicketTypeId { get; set; }

        [Required]
        public decimal TicketPrice { get; set; }
        [Required]
        public Guid OrderDetailId { get; set; }
    }
}
