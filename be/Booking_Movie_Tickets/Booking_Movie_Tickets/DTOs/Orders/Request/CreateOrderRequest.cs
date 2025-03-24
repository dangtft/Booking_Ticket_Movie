namespace Booking_Movie_Tickets.DTOs.Orders.Request
{
    public class CreateOrderRequest
    {
        public string UserId { get; set; }
        public List<OrderDetailRequest> OrderDetailRequests { get; set; }
    }
}
