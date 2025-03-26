using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.DTOs.Authentication.Request;
using Booking_Movie_Tickets.DTOs.Others;
using System.Threading.Tasks;

namespace Booking_Movie_Tickets.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerRequestDTO)
        {
            var result = await _accountService.RegisterAsync(registerRequestDTO);

            if (result is string && !string.IsNullOrEmpty(result.ToString()))
            {
                return BadRequest(new { status = 400, message = result });
            }

            return Ok(new { status = 200, message = "Đăng ký thành công" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginRequestDTO)
        {
            var response = await _accountService.LoginAsync(loginRequestDTO);
            return response != null ? Ok(response) : Unauthorized(ApiMessages.UNAUTHORIZED);
        }

        [HttpPost("social-login")]
        public async Task<IActionResult> SaveSocialUser([FromBody] SocialUserDTO socialUserDto)
        {
            var success = await _accountService.SaveSocialUserAsync(socialUserDto);
            if (!success)
            {
                return BadRequest(new { status = 400, message = "Đăng nhập bằng mạng xã hội thất bại" });
            }

            return Ok(new { status = 200, message = "Đăng nhập bằng mạng xã hội thành công" });
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO request)
        {
            var success = await _accountService.ChangePasswordAsync(request);
            if (!success)
            {
                return BadRequest(new { status = 400, message = "Đổi mật khẩu thất bại" });
            }

            return Ok(new { status = 200, message = "Đổi mật khẩu thành công" });
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserDTO updateUserDTO)
        {
            var success = await _accountService.UpdateUserInfoAsync(updateUserDTO);
            if (!success)
            {
                return BadRequest(new { status = 400, message = "Cập nhật thông tin thất bại" });
            }

            return Ok(new { status = 200, message = "Cập nhật thông tin thành công" });
        }
    }
}
