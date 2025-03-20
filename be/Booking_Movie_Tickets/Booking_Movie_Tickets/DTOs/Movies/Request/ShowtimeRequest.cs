namespace Booking_Movie_Tickets.DTOs.Movies.Request
{
    public class ShowtimeRequest
    {
        public Guid RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public decimal Price { get; set; }
    }
}
