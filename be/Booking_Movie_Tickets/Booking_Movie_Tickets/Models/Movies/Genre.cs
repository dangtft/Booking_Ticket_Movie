using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.Models.Movies
{
    public class Genre
    {
        [Key]
        public Guid Id { get; set; } 

        [Required, MaxLength(100)]
        public string GenreName { get; set; }

        public ICollection<MovieGenre> MovieGenres { get; set; }
    }
}
