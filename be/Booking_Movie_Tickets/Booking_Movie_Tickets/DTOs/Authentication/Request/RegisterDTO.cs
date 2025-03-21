using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.DTOs.Authentication.Request
{
    public class RegisterDTO
    {
        [Required(ErrorMessage ="UserName is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirm password do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
