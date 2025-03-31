namespace Booking_Movie_Tickets.DTOs.Orders.Response
{
    public class OrderResponse
    {
        public Guid OrderId { get; set; } 

        public decimal? TotalAmount { get; set; } 

        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
