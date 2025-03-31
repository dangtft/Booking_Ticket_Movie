using Booking_Movie_Tickets.DTOs.Orders.Request;
using Booking_Movie_Tickets.DTOs.Orders.Response;
using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking_Movie_Tickets.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(ApiMessages.ERROR);
            }

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            if (orders == null || !orders.Any())
            {
                return NotFound(ApiMessages.NOT_FOUND);
            }

            return Ok(orders);
        }

        [HttpGet("{orderId}/tickets")]
        public async Task<IActionResult> GetTicketsByOrderId(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                return BadRequest(ApiMessages.ERROR);
            }

            var tickets = await _orderService.GetTicketsByOrderIdAsync(orderId);

            if (tickets == null || tickets.Count == 0)
            {
                return NotFound(ApiMessages.NOT_FOUND);
            }

            return Ok(tickets);
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            var result = await _orderService.CreateOrderAsync(request);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(ApiMessages.ERROR);
            }
        }
    }
}
