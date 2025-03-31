using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.DTOs.Authentication.Request;
using Booking_Movie_Tickets.DTOs.Others;
using Microsoft.AspNetCore.Identity;
using Booking_Movie_Tickets.Models.Users;

namespace Booking_Movie_Tickets.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<User> _userManager;

        public AccountController(IAccountService accountService, UserManager<User> userManager)
        {
            _accountService = accountService;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerRequestDTO)
        {
            var result = await _accountService.RegisterAsync(registerRequestDTO);

            if (result is string && !string.IsNullOrEmpty(result.ToString()))
            {
                return BadRequest(new { status = 400, message = result });
            }

            return Ok(new { status = 200, message = ApiMessages.SUCCESS });
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
                return BadRequest(new { status = 400, message = ApiMessages.ERROR });
            }

            return Ok(new { status = 200, message = ApiMessages.SUCCESS });
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "User,Admin")]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO request)
        {
            var success = await _accountService.ChangePasswordAsync(request);
            if (!success)
            {
                return BadRequest(new { status = 400, message = ApiMessages.ERROR});
            }

            return Ok(new { status = 200, message = ApiMessages.SUCCESS });
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "User,Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserDTO updateUserDTO)
        {
            var success = await _accountService.UpdateUserInfoAsync(updateUserDTO);
            if (!success)
            {
                return BadRequest(new { status = 400, message = ApiMessages.ERROR });
            }

            return Ok(new { status = 200, message = ApiMessages.SUCCESS });
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "User,Admin")]
        [HttpGet("{userId}")]
        [Authorize] 
        public async Task<IActionResult> GetUserById(string userId)
        {
            var userInfo = await _accountService.GetUserInfoByIdAsync(userId);
            if (userInfo == null)
            {
                return NotFound(ApiMessages.NOT_FOUND);
            }

            return Ok(userInfo);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound(ApiMessages.NOT_FOUND);

            var decodedToken = Uri.UnescapeDataString(token);
            var confirmResult = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if(!confirmResult.Succeeded)
            {
                return BadRequest(ApiMessages.INVALID_REQUEST);
            }

            return Ok(ApiMessages.SUCCESS);
        }

    }
}
