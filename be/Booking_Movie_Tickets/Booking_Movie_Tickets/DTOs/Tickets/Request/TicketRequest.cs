namespace Booking_Movie_Tickets.DTOs.Tickets.Request
{
    public class TicketRequest
    {
        public string UserId { get; set; } = string.Empty;
        public Guid ShowtimeId { get; set; }
        public Guid SeatId { get; set; }
        public Guid TicketTypeId { get; set; }
        public Guid TicketStatusId { get; set; }
    }
}
