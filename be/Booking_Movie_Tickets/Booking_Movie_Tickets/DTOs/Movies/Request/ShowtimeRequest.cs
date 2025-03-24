namespace Booking_Movie_Tickets.DTOs.Movies.Request
{
    public class ShowtimeRequest
    {
        public Guid RoomId { get; set; }
        public DateOnly Date {  get; set; }
        public TimeOnly Time { get; set; }
        public decimal Price { get; set; }
    }
}
