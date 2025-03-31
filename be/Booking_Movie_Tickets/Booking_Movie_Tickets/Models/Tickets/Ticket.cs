using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Booking_Movie_Tickets.Models.Rooms;
using Booking_Movie_Tickets.Models.Movies;
using Newtonsoft.Json;
using Booking_Movie_Tickets.Models.Orders;

namespace Booking_Movie_Tickets.Models.Tickets
{
    public class Ticket
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ShowTimeId { get; set; }
        public Guid OrderDetailId { get; set; }

        public string QRCode { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid SeatId { get; set; }

        public Guid TicketStatusId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TicketPrice { get; set; }

        [JsonIgnore]
        [ForeignKey("ShowTimeId")]
        public virtual Showtime Showtime { get; set; }
        [JsonIgnore]
        [ForeignKey("SeatId")]
        public virtual Seat Seat { get; set; }

        [JsonIgnore]
        [ForeignKey("TicketStatusId")]
        public virtual TicketStatus TicketStatus { get; set; }

        [ForeignKey("OrderDetailId")]
        public virtual OrderDetail OrderDetail { get; set; }
    }

}
