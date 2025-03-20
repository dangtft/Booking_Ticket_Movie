using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.DTOs.Seats
{
    public class ConfirmOrderRequest
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public Guid ShowtimeId { get; set; }

        [Required]
        public List<Guid> SelectedSeats { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Total price must be greater than zero.")]
        public decimal TotalPrice { get; set; }
        public Guid TicketTypeId { get; set; }  
        public Guid TicketStatusId { get; set; } 
        public decimal TicketPrice { get; set; }

        public List<SelectedExtra> Extras { get; set; }
    }
}
