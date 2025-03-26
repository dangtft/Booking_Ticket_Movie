using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Movies;
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
            var result = await _movieMediaService.GetAllMovieMediaAsync(filter);
            return Ok(result);
        }

        // Lấy MovieMedia theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var media = await _movieMediaService.GetMovieMediaByIdAsync(id);
            if (media == null)
                return NotFound("Không tìm thấy Media!");

            return Ok(media);
        }

        // Lấy MovieMedia theo MovieID
        [HttpGet("get-by-movie/{movieId}")]
        public async Task<IActionResult> GetByMovieId(Guid movieId)
        {
            var media = await _movieMediaService.GetMovieMediaByMovieIdAsync(movieId);
            if (media == null)
                return NotFound("Không tìm thấy Media!");

            return Ok(media);
        }

        #region CRUD
        // Thêm MovieMedia mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MovieMedia media)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdMedia = await _movieMediaService.CreateMovieMediaAsync(media);
            return CreatedAtAction(nameof(GetById), new { id = createdMedia.Id }, createdMedia);
        }

        // Cập nhật MovieMedia
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MovieMedia media)
        {
            var updatedMedia = await _movieMediaService.UpdateMovieMediaAsync(id, media);
            if (updatedMedia == null)
                return NotFound("Không tìm thấy Media để cập nhật!");

            return Ok(updatedMedia);
        }

        // Xóa MovieMedia
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var isDeleted = await _movieMediaService.DeleteMovieMediaAsync(id);
            if (!isDeleted)
                return NotFound("Không tìm thấy Media để xóa!");

            return NoContent();
        }
        #endregion
    }
}
