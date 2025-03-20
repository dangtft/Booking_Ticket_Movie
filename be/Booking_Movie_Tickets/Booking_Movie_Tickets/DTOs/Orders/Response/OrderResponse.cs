namespace Booking_Movie_Tickets.DTOs.Orders.Response
{
    public class OrderResponse
    {
        public Guid OrderId { get; set; } 

        public decimal TotalAmount { get; set; } 

        public string Message { get; set; }
    }
}
