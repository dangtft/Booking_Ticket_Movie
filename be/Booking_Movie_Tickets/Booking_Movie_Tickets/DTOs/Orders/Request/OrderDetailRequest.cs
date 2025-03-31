using Booking_Movie_Tickets.DTOs.Extras.Request;
using Booking_Movie_Tickets.DTOs.Tickets.Request;
using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.DTOs.Orders.Request
{
    public class OrderDetailRequest
    {
        [Required]
        public List<OrderTicketRequest> TicketRequests { get; set; }
        public List<OrderExtraRequest>? Extras { get; set; } = new List<OrderExtraRequest>();
    }
}
