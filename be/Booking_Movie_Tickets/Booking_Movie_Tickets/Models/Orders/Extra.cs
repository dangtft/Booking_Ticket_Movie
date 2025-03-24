using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.Models.Orders
{
    public class Extra
    {
        [Key]
        public Guid Id { get; set; } 

        [Required, MaxLength(256)]
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public Guid CategoryId { get; set; }
        public bool IsDeleted { get; set; }
        public string? ImageURL { get; set; }

        [ForeignKey("CategoryId")]
        public virtual ExtrasCategory Category { get; set; }

        public ICollection<OrderDetailExtras> OrderDetailExtras { get; set; } = new List<OrderDetailExtras>();

    }

}
