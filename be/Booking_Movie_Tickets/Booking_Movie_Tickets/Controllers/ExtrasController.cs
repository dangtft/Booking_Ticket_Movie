using Booking_Movie_Tickets.DTOs.Extras.Request;
using Booking_Movie_Tickets.DTOs.Movies.Response;
using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Orders;
using Booking_Movie_Tickets.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking_Movie_Tickets.Controllers
{
    [Route("api/extras")]
    [ApiController]
    public class ExtrasController : ControllerBase
    {
        private readonly IExtraService _extraService;

        public ExtrasController(IExtraService extraService)
        {
            _extraService = extraService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExtraById(Guid id)
        {
            var extra = await _extraService.GetExtraByIdAsync(id);
            if (extra == null) return NotFound(ApiMessages.NOT_FOUND);
            return Ok(extra);
        }

        [HttpGet("price/{id}")]
        public async Task<IActionResult> GetPrice(Guid id)
        {
            var price = await _extraService.GetPriceAsync(id);
            return Ok(price);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExtras([FromQuery] PagedFilterBase filter)
        {
            if (filter.Page < 1 || filter.PageSize < 1)
            {
                return BadRequest(new ApiResponse<string>(ApiMessages.INVALID_PAGINATION));
            }

            try
            {
                var pagedResult = await _extraService.GetAllExtrasAsync(filter);

                if (pagedResult == null || !pagedResult.Data.Any())
                {
                    return NoContent();
                }

                return Ok(new ApiResponse<Extra>(pagedResult));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(ApiMessages.ERROR));
            }
            //var result = await _extraService.GetAllExtrasAsync(filter);
            //return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateExtra([FromBody] ExtraRequest extra)
        {
            var createdExtra = await _extraService.CreateExtraAsync(extra);
            return CreatedAtAction(nameof(GetExtraById), new { id = createdExtra.Id }, createdExtra);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExtra(Guid id, [FromBody] ExtraRequest updatedExtra)
        {
            var success = await _extraService.UpdateExtraAsync(id, updatedExtra);
            if (!success) return NotFound(ApiMessages.NOT_FOUND);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExtra(Guid id)
        {
            var success = await _extraService.DeleteExtraAsync(id);
            if (!success) return NotFound(ApiMessages.NOT_FOUND);
            return NoContent();
        }
    }
}
