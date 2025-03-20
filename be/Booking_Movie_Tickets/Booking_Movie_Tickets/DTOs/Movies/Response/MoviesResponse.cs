using Booking_Movie_Tickets.Models.Movies;

namespace Booking_Movie_Tickets.DTOs.Movies.Response
{
    public class MoviesResponse
    {
        public Guid MovieId { get; set; }
        public string Title { get; set; }
        public string Nation { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Status { get; set; }
        public float Rating { get; set; }
        public List<string> Genres { get; set; }
        public List<string> ImageMovie { get; set; }
    }
}
