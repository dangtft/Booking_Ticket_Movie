using Booking_Movie_Tickets.DTOs.Movies.Response;
using Booking_Movie_Tickets.DTOs.Orders.Request;
using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.DTOs.Seats;
using Booking_Movie_Tickets.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Booking_Movie_Tickets.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IOrderService _orderService;

        public BookingController(IBookingService bookingService, IOrderService orderService)
        {
            _bookingService = bookingService;
            _orderService = orderService;
        }

        [HttpGet("showtimes/{movieId}")]
        public async Task<IActionResult> GetMovieShowtimes(Guid movieId)
        {
            var movieShowtime = await _bookingService.GetShowtimesByMovieId(movieId);

            if (movieShowtime == null)
            {
                return NotFound(ApiResponse<string>.ErrorResponse(ApiMessages.NOT_FOUND));
            }

            return Ok(ApiResponse<List<ShowtimeResponse>>.SuccessResponse((List<ShowtimeResponse>)movieShowtime, ApiMessages.SUCCESS));
        }


        [HttpGet("seats/{showtimeId}")]
        public async Task<IActionResult> GetSeatsByShowtime(Guid showtimeId)
        {
            var seats = await _bookingService.GetSeatsByShowtime(showtimeId);

            if (seats == null || seats.Count == 0)
            {
                return NotFound(ApiResponse<string>.ErrorResponse(ApiMessages.NOT_FOUND));
            }

            return Ok(ApiResponse<List<SeatResponse>>.SuccessResponse(seats));
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            var result = await _orderService.CreateOrderAsync(request);
            if (result)
                return Ok(ApiMessages.SUCCESS);
            return BadRequest(ApiMessages.ERROR);
        }

        [HttpPost("cleanup-unpaid")]
        public async Task<IActionResult> CleanupUnpaidOrders()
        {
            await _orderService.CleanupUnpaidOrdersAsync();
            return Ok(ApiMessages.SUCCESS);
        }
    }
}