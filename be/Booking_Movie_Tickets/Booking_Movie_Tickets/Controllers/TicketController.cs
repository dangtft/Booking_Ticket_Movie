using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.DTOs.Tickets.Request;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Orders;
using Booking_Movie_Tickets.Models.Tickets;
using Booking_Movie_Tickets.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking_Movie_Tickets.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpPost]
        public IActionResult CreateTicket([FromBody] TicketRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ticket = _ticketService.CreateTicket(request);
            return Ok(ticket);
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpGet("{ticketId}/qrcode")]
        public async Task<IActionResult> GetQRCode(Guid ticketId)
        {
            var qrCode = await _ticketService.GetQRCodeAsync(ticketId);

            if (string.IsNullOrEmpty(qrCode))
            {
                return NotFound(ApiMessages.NOT_FOUND);
            }

            return Content(qrCode, "image/svg+xml"); 
        }

    }
}
