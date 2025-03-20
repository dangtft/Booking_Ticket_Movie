using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.Models.Movies
{
    public class AgeRating
    {
        [Key]
        public Guid Id { get; set; } 

        [Required, MaxLength(10)]
        public string RatingLabel { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }
    }
}
