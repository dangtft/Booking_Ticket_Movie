namespace Booking_Movie_Tickets.DTOs.Authentication.Response
{
    public class UserInfoDTO
    {
        public string UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
