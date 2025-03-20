using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.Interfaces;
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

        [HttpGet]
        public async Task<IActionResult> GetAllExtras([FromQuery] PagedFilterBase filter)
        {
            var result = await _extraService.GetAllExtrasAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetExtraById(Guid id)
        {
            var extra = _extraService.GetExtraById(id);
            if (extra == null)
            {
                return NotFound(new { Message = "Extra not found" });
            }
            return Ok(extra);
        }

        [HttpGet("{id}/price")]
        public IActionResult GetPrice(Guid id)
        {
            var price = _extraService.GetPrice(id);
            return Ok(new { Price = price });
        }
    }
}
