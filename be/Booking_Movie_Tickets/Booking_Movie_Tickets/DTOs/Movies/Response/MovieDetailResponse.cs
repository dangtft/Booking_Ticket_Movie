namespace Booking_Movie_Tickets.DTOs.Movies.Response
{
    public class MovieDetailResponse
    {
        public Guid MovieId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Nation { get; set; }
        public string Status { get; set; }
        public int Duration { get; set; }
        public float Rating { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string AgeRating { get; set; }
        public object Genres { get; set; }
    }
}
