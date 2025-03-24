using Booking_Movie_Tickets.DTOs.Authentication.Request;
using Booking_Movie_Tickets.DTOs.Authentication.Response;
using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace Booking_Movie_Tickets.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public AccountService(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<object> RegisterAsync(RegisterDTO registerRequestDTO)
        {
            if (registerRequestDTO == null)
            {
                return ApiMessages.ERROR;
            }

            var existingUser = await _userManager.FindByEmailAsync(registerRequestDTO.UserName);
            if (existingUser != null)
            {
                return ApiMessages.ERROR;
            }

            var identityUser = new User
            {
                UserName = registerRequestDTO.UserName,
                Email = registerRequestDTO.Email,
            };

            var identityResult = await _userManager.CreateAsync(identityUser, registerRequestDTO.Password);
            if (!identityResult.Succeeded)
            {
                return identityResult.Errors.Select(e => e.Description);
            }

            return ApiMessages.SUCCESS;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginDTO loginRequestDTO)
        {
            if (loginRequestDTO == null)
            {
                return null;
            }

            var user = await _userManager.FindByNameAsync(loginRequestDTO.UserName)
                      ?? await _userManager.FindByEmailAsync(loginRequestDTO.UserName);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password))
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var jwtToken = _tokenService.CreateJWTToken(user, roles.ToList());

            return new LoginResponseDTO { JwtToken = jwtToken };
        }

        public async Task<bool> SaveSocialUserAsync(SocialUserDTO socialUserDto)
        {
            if (socialUserDto == null || string.IsNullOrEmpty(socialUserDto.Email))
            {
                return false;
            }

            var user = await _userManager.FindByEmailAsync(socialUserDto.Email);

            if (user == null)
            {
                user = new User
                {
                    UserName = socialUserDto.Email,
                    Email = socialUserDto.Email,
                    FullName = socialUserDto.FullName,
                    AvatarUrl = socialUserDto.AvatarUrl,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user);
                return result.Succeeded;
            }
            else
            {
                user.FullName = socialUserDto.FullName;
                user.AvatarUrl = socialUserDto.AvatarUrl;
                await _userManager.UpdateAsync(user);
                return true;
            }
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordRequestDTO request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return false;
            }

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
            if (!isPasswordCorrect)
            {
                return false; 
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            return changePasswordResult.Succeeded;
        }

        public async Task<bool> UpdateUserInfoAsync(UpdateUserDTO updateUserDTO)
        {
            var user = await _userManager.FindByIdAsync(updateUserDTO.UserId);
            if (user == null)
            {
                return false;
            }

            user.FullName = updateUserDTO.FullName ?? user.FullName;
            user.AvatarUrl = updateUserDTO.AvatarUrl ?? user.AvatarUrl;
            user.PhoneNumber = updateUserDTO.PhoneNumber ?? user.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
