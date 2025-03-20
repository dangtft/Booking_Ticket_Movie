using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Booking_Movie_Tickets.Models.Tickets;

namespace Booking_Movie_Tickets.Models.Orders
{
    public class OrderDetail
    {
        [Key]
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid TicketId { get; set; }

        public Guid? ExtraId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName =" decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [ForeignKey("TicketId")]
        public virtual Ticket Ticket { get; set; }

        [ForeignKey("ExtraId")]
        public virtual Extra? Extra { get; set; }
    }

}
