namespace Booking_Movie_Tickets.DTOs.Seats.Request
{
    public class LocketSeatRequest
    {
        public string UserId { get; set; }
        public Guid SeatId { get; set; }
        public Guid ShowTimeId { get; set; }
    }
}
