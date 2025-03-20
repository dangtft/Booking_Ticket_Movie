namespace Booking_Movie_Tickets.DTOs.Tickets.Response
{
    public class TicketResponse
    {
        public Guid TicketId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public string Message { get; set; } = "Booking successful!";
    }
}
