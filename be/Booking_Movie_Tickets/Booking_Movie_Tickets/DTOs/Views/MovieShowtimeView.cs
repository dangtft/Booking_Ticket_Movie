namespace Booking_Movie_Tickets.DTOs.Views
{
    public class MovieShowtimeView
    {
        public Guid MovieId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Nation { get; set; }
        public int Duration { get; set; }
        public float Rating { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string GenreName { get; set; }
        public string AgeRating { get; set; }
        public DateTime StartTime { get; set; }
        public decimal Price { get; set; }
        public string RoomName { get; set; }
    }
}
