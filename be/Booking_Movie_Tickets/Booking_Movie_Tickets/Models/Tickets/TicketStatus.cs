using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.Models.Tickets
{
    public class TicketStatus
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string StatusName { get; set; }
    }

}
