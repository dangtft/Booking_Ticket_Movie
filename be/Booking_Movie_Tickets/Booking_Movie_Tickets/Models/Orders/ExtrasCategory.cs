using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.Models.Orders
{
    public class ExtrasCategory
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string CategoryName { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Extra> Extras { get; set; }
    }

}
