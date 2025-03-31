using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Movies;
using Booking_Movie_Tickets.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking_Movie_Tickets.Controllers
{
    [Route("api/movie-medias")]
    [ApiController]
    public class MovieMediaController : ControllerBase
    {
        private readonly IMovieMediaService _movieMediaService;

        public MovieMediaController(IMovieMediaService movieMediaService)
        {
            _movieMediaService = movieMediaService;
        }

        // Lấy toàn bộ MovieMedia
        [HttpGet]
        public async Task<IActionResult> GetAllMovieMedia([FromQuery] PagedFilterBase filter)
        {
            if (filter.Page < 1 || filter.PageSize < 1)
            {
                return BadRequest(new ApiResponse<string>(ApiMessages.INVALID_PAGINATION));
            }

            try
            {
                var pagedResult = await _movieMediaService.GetAllMovieMediaAsync(filter);

                if (pagedResult == null || !pagedResult.Data.Any())
                {
                    return NoContent();
                }

                return Ok(new ApiResponse<MovieMedia>(pagedResult));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(ApiMessages.ERROR));
            }
        }

        // Lấy MovieMedia theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var media = await _movieMediaService.GetMovieMediaByIdAsync(id);
            if (media == null)
                return NotFound(ApiMessages.NOT_FOUND);

            return Ok(media);
        }

        // Lấy MovieMedia theo MovieID
        [HttpGet("get-by-movie/{movieId}")]
        public async Task<IActionResult> GetByMovieId(Guid movieId)
        {
            var media = await _movieMediaService.GetMovieMediaByMovieIdAsync(movieId);
            if (media == null)
                return NotFound(ApiMessages.NOT_FOUND);

            return Ok(media);
        }

        #region CRUD
        // Thêm MovieMedia mới
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MovieMedia media)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdMedia = await _movieMediaService.CreateMovieMediaAsync(media);
            return Ok(ApiMessages.CREATED_SUCCESS);
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MovieMedia media)
        {
            var updatedMedia = await _movieMediaService.UpdateMovieMediaAsync(id, media);
            if (updatedMedia == null)
                return NotFound(ApiMessages.NOT_FOUND);

            return Ok(ApiMessages.UPDATED_SUCCESS);
        }

        // Xóa MovieMedia
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var isDeleted = await _movieMediaService.DeleteMovieMediaAsync(id);
            if (!isDeleted)
                return NotFound(ApiMessages.NOT_FOUND);

            return Ok(ApiMessages.DELETED_SUCCESS);
        }
        #endregion
    }
}
