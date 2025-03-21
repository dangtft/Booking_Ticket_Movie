using Booking_Movie_Tickets.DTOs.Authentication.Request;
using Booking_Movie_Tickets.DTOs.Authentication.Response;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface IAccountService
    {
        Task<object> RegisterAsync(RegisterDTO registerRequestDTO);
        Task<LoginResponseDTO> LoginAsync(LoginDTO loginRequestDTO);
        Task<bool> SaveSocialUserAsync(SocialUserDTO socialUserDto);
        Task<bool> ChangePasswordAsync(ChangePasswordRequestDTO request);
        Task<bool> UpdateUserInfoAsync(UpdateUserDTO updateUserDTO);
    }
}
