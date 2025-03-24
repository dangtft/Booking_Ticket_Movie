using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.Models.Tickets
{
    public class TicketType
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string TypeName { get; set; }

        public decimal Discount { get; set; }
        public string Type { get; set; }
    }

}
