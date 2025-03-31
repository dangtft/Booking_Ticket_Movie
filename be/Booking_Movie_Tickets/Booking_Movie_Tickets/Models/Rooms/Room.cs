using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking_Movie_Tickets.Models.Rooms
{
    public class Room
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(256)]
        public string Name { get; set; }
        public int TotalSeats { get; set; }
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }

}
