using Microsoft.AspNetCore.Identity;

namespace Booking_Movie_Tickets.Models.Users
{
    public class User : IdentityUser
    {
        public bool IsDeleted { get; set; }
        public string? RefreshToken { get; set; }
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
