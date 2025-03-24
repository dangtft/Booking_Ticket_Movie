namespace Booking_Movie_Tickets.DTOs.Orders.Request
{
    public class ProcessPaymentRequest
    {
        public Guid OrderId { get; set; }
        public bool IsSuccess { get; set; }
    }
}
