namespace Booking_Movie_Tickets.DTOs.Authentication.Request
{
    public class UpdateUserDTO
    {
        public string UserId { get; set; }
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
