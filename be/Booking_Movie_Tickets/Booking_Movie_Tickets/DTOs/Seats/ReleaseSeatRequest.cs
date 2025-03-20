namespace Booking_Movie_Tickets.DTOs.Seats
{
    public class ReleaseSeatRequest
    {
        public Guid SeatId { get; set; }
        public Guid ShowtimeId { get; set; }
    }
}
