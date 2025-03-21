using Booking_Movie_Tickets.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface ITokenService
    {
        string CreateJWTToken(User user, List<string> roles);
    }
}
