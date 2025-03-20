namespace Booking_Movie_Tickets.Models.Movies
{
    public class MovieActor
    {
        public Guid MovieId { get; set; }
        public Movie Movie { get; set; }

        public Guid ActorId { get; set; }
        public Actor Actor { get; set; }

        public string Role { get; set; }
    }
}
