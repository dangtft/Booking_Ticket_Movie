namespace Booking_Movie_Tickets.DTOs.Authentication.Request
{
    public class ChangePasswordRequestDTO
    {
        public string UserName { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
