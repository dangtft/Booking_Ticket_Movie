using Booking_Movie_Tickets.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking_Movie_Tickets.Models
{
    public class ChangeHistory
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string Table_Name { get; set; }

        public Guid Record_Id { get; set; }

        public string Change_Type { get; set; } 

        public string Old_Data { get; set; }  

        public string New_Data { get; set; }  

        public string Change_By { get; set; } 

        public DateTime Change_At { get; set; }

        [ForeignKey("Change_By")]
        public User ChangedBy { get; set; }
    }

}
