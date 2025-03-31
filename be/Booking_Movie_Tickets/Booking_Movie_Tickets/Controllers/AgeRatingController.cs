using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Movies;
using Microsoft.AspNetCore.Mvc;
using Booking_Movie_Tickets.DTOs.Movies.Request;
using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.Services;
using Microsoft.AspNetCore.Authorization;

namespace Booking_Movie_Tickets.Controllers
{
    [Route("api/age-ratings")]
    [ApiController]
    public class AgeRatingController : ControllerBase
    {
        private readonly IAgeRatingService _ageRatingService;

        public AgeRatingController(IAgeRatingService ageRatingService)
        {
            _ageRatingService = ageRatingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAgeRatings([FromQuery] AgeRatingFilter filter)
        {
            if (filter.Page < 1 || filter.PageSize < 1)
            {
                return BadRequest(new ApiResponse<string>(ApiMessages.INVALID_PAGINATION));
            }

            try
            {
                var pagedResult = await _ageRatingService.GetAllAgeRatingsAsync(filter);

                if (pagedResult == null || !pagedResult.Data.Any())
                {
                    return NoContent();
                }

                return Ok(new ApiResponse<AgeRating>(pagedResult));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(ApiMessages.ERROR));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ageRating = await _ageRatingService.GetAgeRatingByIdAsync(id);
            if (ageRating == null)
            {
                return NotFound(ApiMessages.NOT_FOUND);
            }
            return Ok(ageRating);
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AgeRatingRequest ageRating)
        {
            if (ageRating == null)
            {
                return BadRequest(ApiMessages.INVALID_REQUEST);
            }

            var created = await _ageRatingService.AddAgeRatingAsync(ageRating);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AgeRatingRequest ageRating)
        {
            if (ageRating == null)
            {
                return BadRequest(ApiMessages.INVALID_REQUEST);
            }

            var updated = await _ageRatingService.UpdateAgeRatingAsync(id, ageRating);
            if (updated == null)
            {
                return NotFound(ApiMessages.NOT_FOUND);
            }

            return Ok(updated);
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _ageRatingService.DeleteAgeRatingAsync(id);
            if (!deleted)
            {
                return NotFound(ApiMessages.NOT_FOUND);
            }

            return Ok(ApiMessages.SUCCESS);
        }
    }
}
