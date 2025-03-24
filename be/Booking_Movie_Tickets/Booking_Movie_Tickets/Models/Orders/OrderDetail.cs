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

        [Column(TypeName =" decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        public ICollection<OrderDetailExtras> OrderDetailExtras { get; set; } = new List<OrderDetailExtras>();
    }

}
