using Booking_Movie_Tickets.DTOs.Movies.Request;
using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Movies;
using Booking_Movie_Tickets.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking_Movie_Tickets.Controllers
{
    [Route("api/genres")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PagedFilterBase filter)
        {
            if (filter.Page < 1 || filter.PageSize < 1)
            {
                return BadRequest(new ApiResponse<string>(ApiMessages.INVALID_PAGINATION));
            }

            try
            {
                var pagedResult = await _genreService.GetAllGenresAsync(filter);

                if (pagedResult == null || !pagedResult.Data.Any())
                {
                    return NoContent();
                }

                return Ok(new ApiResponse<Genre>(pagedResult));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(ApiMessages.ERROR));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var genre = await _genreService.GetGenreByIdAsync(id);
            if (genre == null)
            {
                return NotFound(ApiMessages.NOT_FOUND);
            }
            return Ok(genre);
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GenreRequest genre)
        {
            if (genre == null)
            {
                return BadRequest(ApiMessages.INVALID_REQUEST);
            }

            var created = await _genreService.AddGenreAsync(genre);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] GenreRequest genre)
        {
            if (genre == null)
            {
                return BadRequest(ApiMessages.INVALID_REQUEST);
            }

            var updated = await _genreService.UpdateGenreAsync(id, genre);
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
            var deleted = await _genreService.DeleteGenreAsync(id);
            if (!deleted)
            {
                return NotFound(ApiMessages.NOT_FOUND);
            }
            return Ok(ApiMessages.SUCCESS);
        }
    }
}