using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Booking_Movie_Tickets.Models.Orders;
using Booking_Movie_Tickets.Models.Users;

namespace Booking_Movie_Tickets.Models.Payments
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public Guid OrderId { get; set; }

        [Column(TypeName ="decimal(18,2)")]
        public decimal Amount { get; set; }

        [MaxLength(256)]
        public Guid TransactionId { get; set; }

        public Guid PaymentMethodId { get; set; }

        public Guid PaymentStatusId { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMethod PaymentMethod { get; set; }

        [ForeignKey("PaymentStatusId")]
        public virtual PaymentStatus PaymentStatus { get; set; }
    }

}
