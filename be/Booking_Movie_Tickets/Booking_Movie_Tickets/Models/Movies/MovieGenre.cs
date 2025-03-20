using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.Models.Movies
{
    public class MovieGenre
    {
        public Guid MovieId { get; set; } 
        public Guid GenreId { get; set; } 

        public Movie Movie { get; set; }
        public Genre Genre { get; set; }
    }

}
