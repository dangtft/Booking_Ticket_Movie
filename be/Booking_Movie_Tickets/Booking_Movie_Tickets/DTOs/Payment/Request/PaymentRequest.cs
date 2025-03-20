namespace Booking_Movie_Tickets.DTOs.Payment.Request
{
    public class PaymentRequest
    {
        public string UserId { get; set; } = string.Empty;
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public Guid PaymentMethodId { get; set; }
        public Guid PaymentStatusId { get; set; }
    }
}
