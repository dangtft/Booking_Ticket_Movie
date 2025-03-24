namespace Booking_Movie_Tickets.DTOs.Movies.Response
{
    public class ShowtimeResponse
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public Guid MovieId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public decimal Price { get; set; }
        public string RoomName { get; set; }
    }
}
