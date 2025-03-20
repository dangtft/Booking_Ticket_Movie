namespace Booking_Movie_Tickets.DTOs.Payment.Response
{
    public class PaymentResponse
    {
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = "Success";
        public string Message { get; set; } = "Payment completed successfully!";
    }
}
