using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking_Movie_Tickets.Models.Rooms
{
    public class SeatType
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string TypeName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceModifier { get; set; }
    }

}
