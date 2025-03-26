using Booking_Movie_Tickets.Models.Cinemas;
using Booking_Movie_Tickets.Models.Movies;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.Models.Rooms
{
    public class SeatStatusTracking
    {

        [Required]
        public Guid SeatId { get; set; }  

        [Required]
        public Guid ShowTimeId { get; set; }  

        [Required]
        [StringLength(50)]
        public string Status { get; set; }
        public bool IsLocked { get; set; } = false;
        public string? LockedByUserId { get; set; }
        public DateTime? LockedAt { get; set; }
        public DateTime? ExpirationTime { get; set; }

        [ForeignKey("SeatId")]
        public Seat Seat { get; set; }

        [ForeignKey("ShowTimeId")]
        public Showtime Showtime { get; set; }

    }
}
