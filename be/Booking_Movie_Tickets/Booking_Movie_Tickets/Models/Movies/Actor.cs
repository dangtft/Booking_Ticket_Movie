namespace Booking_Movie_Tickets.Models.Movies
{
    public class Actor
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string? Details { get; set; }
        public string? ImageURL { get; set; }

        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    }
}
