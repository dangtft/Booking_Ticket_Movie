using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Booking_Movie_Tickets.Models.Cinemas;
using Booking_Movie_Tickets.Models.Movies;
using Booking_Movie_Tickets.Models.Users;

namespace Booking_Movie_Tickets.Models.Tickets
{
    public class Ticket
    {
        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public Guid ShowTimeId { get; set; }

        [Required, MaxLength(50)]
        public string BookingCode { get; set; }

        public Guid SeatId { get; set; }

        public Guid TicketTypeId { get; set; }

        public Guid TicketStatusId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TicketPrice { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("ShowTimeId")]
        public virtual Showtime Showtime { get; set; }

        [ForeignKey("SeatId")]
        public virtual Seat Seat { get; set; }

        [ForeignKey("TicketTypeId")]
        public virtual TicketType TicketType { get; set; }

        [ForeignKey("TicketStatusId")]
        public virtual TicketStatus TicketStatus { get; set; }
    }

}
