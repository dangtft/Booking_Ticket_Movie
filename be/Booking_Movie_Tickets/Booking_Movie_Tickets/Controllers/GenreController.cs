using Booking_Movie_Tickets.DTOs.Movies.Request;
using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Booking_Movie_Tickets.Controllers
{
    [Route("api/genres")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;
        private readonly ILogger<GenreController> _logger;

        public GenreController(IGenreService genreService, ILogger<GenreController> logger)
        {
            _genreService = genreService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PagedFilterBase filter)
        {
            var result = await _genreService.GetAllGenresAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation($"Lấy thể loại phim với ID: {id}");
            var genre = await _genreService.GetGenreByIdAsync(id);
            if (genre == null)
            {
                _logger.LogWarning($"Không tìm thấy thể loại phim với ID: {id}");
                return NotFound($"Không có thể loại phim nào được tìm thấy với ID: {id}");
            }
            return Ok(genre);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GenreRequest genre)
        {
            if (genre == null)
            {
                _logger.LogError("Dữ liệu thể loại phim không hợp lệ.");
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            var created = await _genreService.AddGenreAsync(genre);
            _logger.LogInformation($"Thể loại phim mới đã được tạo với ID: {created.Id}");
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] GenreRequest genre)
        {
            if (genre == null)
            {
                _logger.LogError("Yêu cầu cập nhật thể loại phim không hợp lệ.");
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            var updated = await _genreService.UpdateGenreAsync(id, genre);
            if (updated == null)
            {
                _logger.LogWarning($"Không tìm thấy thể loại phim để cập nhật với ID: {id}");
                return NotFound($"Không tìm thấy thể loại phim với ID: {id}");
            }

            _logger.LogInformation($"Đã cập nhật thể loại phim với ID: {id}");
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation($"Đang cố gắng xóa thể loại phim với ID: {id}");
            var deleted = await _genreService.DeleteGenreAsync(id);
            if (!deleted)
            {
                _logger.LogWarning($"Xóa thất bại, không tìm thấy thể loại phim với ID: {id}");
                return NotFound($"Không tìm thấy thể loại phim với ID: {id}");
            }

            _logger.LogInformation($"Đã xóa thể loại phim với ID: {id}");
            return Ok($"Thể loại phim với ID {id} đã bị xóa.");
        }
    }
}