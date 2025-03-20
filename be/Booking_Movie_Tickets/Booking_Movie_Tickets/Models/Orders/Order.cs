using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Booking_Movie_Tickets.Models.Users;

namespace Booking_Movie_Tickets.Models.Orders
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

}
