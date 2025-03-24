using Booking_Movie_Tickets.DTOs.Movies.Request;
using Booking_Movie_Tickets.DTOs.Movies.Response;
using Booking_Movie_Tickets.DTOs.Others;

using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Movies;
using Microsoft.AspNetCore.Mvc;

namespace Booking_Movie_Tickets.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMovies([FromQuery] MovieFilter movieFilter)
        {
            if (movieFilter.Page < 1 || movieFilter.PageSize < 1)
            {
                return BadRequest(new ApiResponse<string>(ApiMessages.INVALID_PAGINATION));
            }

            try
            {
                var pagedResult = await _movieService.GetMoviesAsync(movieFilter);

                if (pagedResult == null || !pagedResult.Data.Any())
                {
                    return NoContent();
                }

                return Ok(new ApiResponse<MoviesResponse>(pagedResult));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(ApiMessages.ERROR));
            }
        }

        [HttpGet("{movieId}")]
        public async Task<IActionResult> GetMovieById(Guid movieId)
        {
            var movie = await _movieService.GetMovieById(movieId);
            if (movie == null)
            {
                return NotFound(ApiMessages.NOT_FOUND);
            }
            return Ok(movie);
        }

        #region CRUD Movie
        [HttpPost]
        public async Task<IActionResult> AddMovie([FromBody] MovieRequest request)
        {
            if (request == null)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ApiMessages.INVALID_REQUEST));
            }

            var createdMovie = await _movieService.AddMovieAsync(request);

            // 🛑 Kiểm tra nếu `createdMovie` null
            if (createdMovie == null || createdMovie.Id == Guid.Empty)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.ErrorResponse(ApiMessages.ERROR));
            }

            return CreatedAtAction(nameof(GetMovieById),
                new { movieId = createdMovie.Id },
                ApiResponse<Movie>.SuccessResponse(createdMovie, ApiMessages.CREATED_SUCCESS));
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(Guid id, [FromBody] MovieRequest request)
        {
            if (request == null)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ApiMessages.INVALID_REQUEST));
            }

            var updatedMovie = await _movieService.UpdateMovieAsync(id, request);

            if (updatedMovie == null)
            {
                return NotFound(ApiResponse<string>.ErrorResponse(ApiMessages.NOT_FOUND));
            }

            return Ok(ApiResponse<Movie>.SuccessResponse(updatedMovie, ApiMessages.UPDATED_SUCCESS));

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(Guid id)
        {
            var deleted = await _movieService.DeleteMovieAsync(id);

            if (!deleted)
            {
                return NotFound(ApiResponse<string>.ErrorResponse(ApiMessages.NOT_FOUND));
            }

            return Ok(ApiResponse<string>.SuccessResponse(ApiMessages.DELETED_SUCCESS));
        }
        #endregion
    }
}
