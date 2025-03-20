using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.Models.Movies
{
    public class MovieMedia
    {
        [Key]
        public Guid Id { get; set; }

        public Guid MovieId { get; set; }

        public string Description { get; set; }

        [Required, MaxLength(50)]
        public string MediaType { get; set; }

        [Required]
        public string MediaURL { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("MovieId")]
        public virtual Movie Movie { get; set; }
    }

}
