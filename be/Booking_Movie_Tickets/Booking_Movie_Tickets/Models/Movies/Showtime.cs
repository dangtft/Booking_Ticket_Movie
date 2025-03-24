using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Booking_Movie_Tickets.Models.Cinemas;

namespace Booking_Movie_Tickets.Models.Movies
{
    public class Showtime
    {
        [Key]
        public Guid Id { get; set; } 

        public Guid MovieId { get; set; }

        public Guid RoomId { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly Time { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(MovieId))]
        public virtual Movie Movie { get; set; }

        [ForeignKey(nameof(RoomId))]
        public virtual Room Room { get; set; }
    }

}
