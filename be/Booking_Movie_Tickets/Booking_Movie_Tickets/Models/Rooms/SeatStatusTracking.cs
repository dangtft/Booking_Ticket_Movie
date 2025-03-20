using Booking_Movie_Tickets.Models.Cinemas;
using Booking_Movie_Tickets.Models.Movies;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.Models.Rooms
{
    public class SeatStatusTracking
    {
        [Key]
        public Guid Id { get; set; }  

        [Required]
        public Guid Seat_Id { get; set; }  

        [Required]
        public Guid Show_Time_Id { get; set; }  

        [Required]
        public Guid Room_Id { get; set; } 

        [Required]
        [StringLength(50)]
        public string Status { get; set; }
        public bool IsLocked { get; set; } = false;
        public string? LockedByUserId { get; set; }
        public DateTime? LockedAt { get; set; }
        public DateTime Updated_At { get; set; } = DateTime.UtcNow;  

        [ForeignKey("Seat_Id")]
        public Seat Seat { get; set; }

        [ForeignKey("Show_Time_Id")]
        public Showtime Showtime { get; set; }

        [ForeignKey("Room_Id")]
        public Room Room { get; set; }
    }
}
