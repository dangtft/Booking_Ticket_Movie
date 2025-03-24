using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.DTOs.Orders.Request
{
    public class OrderRequest
    {
        [Required]
        public string UserId { get; set; }
        public List<OrderDetailRequest> OrderDetails { get; set; } = new List<OrderDetailRequest>();
    }
}
