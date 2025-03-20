using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Movies;
using Microsoft.AspNetCore.Mvc;
using Booking_Movie_Tickets.DTOs.Movies.Request;
using Booking_Movie_Tickets.DTOs.Others;

namespace Booking_Movie_Tickets.Controllers
{
    [Route("api/age-ratings")]
    [ApiController]
    public class AgeRatingController : ControllerBase
    {
        private readonly IAgeRatingService _ageRatingService;
        private readonly ILogger<AgeRatingController> _logger;

        public AgeRatingController(IAgeRatingService ageRatingService, ILogger<AgeRatingController> logger)
        {
            _ageRatingService = ageRatingService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAgeRatings([FromQuery] AgeRatingFilter filter)
        {
            try
            {
                var result = await _ageRatingService.GetAllAgeRatingsAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation($"Lấy dữ liệu phân loại độ tuổi với ID: {id}");
            var ageRating = await _ageRatingService.GetAgeRatingByIdAsync(id);
            if (ageRating == null)
            {
                _logger.LogWarning($"Không tìm thấy phân loại độ tuổi với ID: {id}");
                return NotFound($"Không có phân loại độ tuổi nào được tìm thấy với ID: {id}");
            }
            return Ok(ageRating);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AgeRatingRequest ageRating)
        {
            if (ageRating == null)
            {
                _logger.LogError("Dữ liệu phân loại độ tuổi không hợp lệ.");
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            var created = await _ageRatingService.AddAgeRatingAsync(ageRating);
            _logger.LogInformation($"Phân loại độ tuổi mới đã được tạo với ID: {created.Id}");
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AgeRatingRequest ageRating)
        {
            if (ageRating == null)
            {
                _logger.LogError("Yêu cầu cập nhật phân loại độ tuổi không hợp lệ.");
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            var updated = await _ageRatingService.UpdateAgeRatingAsync(id, ageRating);
            if (updated == null)
            {
                _logger.LogWarning($"Không tìm thấy phân loại độ tuổi để cập nhật với ID: {id}");
                return NotFound($"Không tìm thấy phân loại độ tuổi với ID: {id}");
            }

            _logger.LogInformation($"Đã cập nhật phân loại độ tuổi với ID: {id}");
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation($"Đang cố gắng xóa phân loại độ tuổi với ID: {id}");
            var deleted = await _ageRatingService.DeleteAgeRatingAsync(id);
            if (!deleted)
            {
                _logger.LogWarning($"Xóa thất bại, không tìm thấy phân loại độ tuổi với ID: {id}");
                return NotFound($"Không tìm thấy phân loại độ tuổi với ID: {id}");
            }

            _logger.LogInformation($"Đã xóa phân loại độ tuổi với ID: {id}");
            return Ok($"Phân loại độ tuổi với ID {id} đã bị xóa.");
        }
    }
}
