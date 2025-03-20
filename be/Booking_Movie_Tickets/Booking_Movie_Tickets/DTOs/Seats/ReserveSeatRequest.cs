namespace Booking_Movie_Tickets.DTOs.Seats
{
    public class ReserveSeatRequest
    {
        public Guid SeatId { get; set; }
        public Guid ShowtimeId { get; set; }
        public string UserId { get; set; }
    }
}
