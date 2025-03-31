using Booking_Movie_Tickets.DTOs.Authentication.Request;
using Booking_Movie_Tickets.DTOs.Authentication.Response;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace Booking_Movie_Tickets.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailSender;

        public AccountService(UserManager<User> userManager, ITokenService tokenService,IEmailSender emailSender)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _emailSender = emailSender;
        }

        public async Task<object> RegisterAsync(RegisterDTO registerRequestDTO)
        {
            if (registerRequestDTO == null)
            {
                return "Dữ liệu không hợp lệ";
            }

            var existingUser = await _userManager.FindByEmailAsync(registerRequestDTO.UserName);
            if (existingUser != null)
            {
                return "Email đã tồn tại";
                
            }

            var identityUser = new User
            {
                UserName = registerRequestDTO.UserName,
                Email = registerRequestDTO.Email,
            };

            var identityResult = await _userManager.CreateAsync(identityUser, registerRequestDTO.Password);
            if (!identityResult.Succeeded)
            {
                return identityResult.Errors.Select(e => e.Description).FirstOrDefault();
            }

            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
            //var confirmationLink = $"https://localhost:7026/api/account/confirm-email?email={identityUser.Email}&token={Uri.EscapeDataString(token)}";

            //// Gửi email xác nhận
            //await _emailSender.SendEmailAsync(identityUser.Email, "Xác nhận Email",
            //    $"Nhấp vào link để xác nhận email: <a href='{confirmationLink}'>Xác nhận</a>");

            return null;
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

        public async Task<UserInfoDTO> GetUserInfoByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            return new UserInfoDTO
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                PhoneNumber = user.PhoneNumber
            };
        }

    }
}
