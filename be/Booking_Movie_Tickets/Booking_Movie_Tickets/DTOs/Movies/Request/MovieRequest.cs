namespace Booking_Movie_Tickets.DTOs.Movies.Request
{
    public class MovieRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Nation { get; set; }
        public int Duration { get; set; }
        public Guid AgeRatingId { get; set; }
        public float Rating { get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<Guid>? GenreIds { get; set; }
        public List<ShowtimeRequest>? Showtimes { get; set; }
        public List<MovieMediaRequest>? MovieMedias { get; set; }
    }
}
