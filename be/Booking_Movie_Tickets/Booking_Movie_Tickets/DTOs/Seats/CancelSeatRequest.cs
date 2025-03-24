namespace Booking_Movie_Tickets.DTOs.Seats
{
    public class CancelSeatRequest
    {
        public string? UserId { get; set; }
        public Guid ShowtimeId { get; set; }
        public Guid SeatId { get; set; }
    }
}
