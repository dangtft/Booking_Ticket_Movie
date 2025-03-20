using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.Models.Movies
{
    public class Movie
    {
        [Key]
        public Guid Id { get; set; } 

        [Required, MaxLength(256)]
        public string Title { get; set; }

        public string Description { get; set; }

        [MaxLength(256)]
        public string? Nation { get; set; }
        [MaxLength(100)]
        public string Status { get; set; }

        public int Duration { get; set; }

        public Guid AgeRatingId { get; set; }

        public float Rating { get; set; }

        public DateTime ReleaseDate { get; set; }

        public bool IsDeleted { get; set; } = false;
        public string SearchData { get; set; }

        [ForeignKey(nameof(AgeRatingId))]
        public virtual AgeRating? AgeRating { get; set; }

        public virtual ICollection<MovieGenre>? MovieGenres { get; set; }
        public virtual ICollection<MovieMedia>? MovieMedias { get; set; }

        public virtual ICollection<Showtime>? Showtimes { get; set; }
        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    }

}
