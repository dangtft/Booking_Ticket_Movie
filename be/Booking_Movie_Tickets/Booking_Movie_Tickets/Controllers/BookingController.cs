using Booking_Movie_Tickets.DTOs.Movies.Response;
using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.DTOs.Seats;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Services;
using Microsoft.AspNetCore.Mvc;

namespace Booking_Movie_Tickets.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("showtimes/{movieId}")]
        public async Task<IActionResult> GetMovieShowtimes(Guid movieId)
        {
            var movieShowtime = await _bookingService.GetShowtimesByMovieId(movieId);

            if (movieShowtime == null)
            {
                return NotFound(ApiResponse<string>.ErrorResponse(ApiMessages.NOT_FOUND));
            }

            return Ok(ApiResponse<List<MoviesResponse>>.SuccessResponse((List<MoviesResponse>)movieShowtime, ApiMessages.SUCCESS));
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

        [HttpPost("select-seats")]
        public IActionResult SelectSeats([FromBody] SelectSeatsRequest request)
        {
            var result = _bookingService.SelectSeats(request.SeatIds, request.ShowtimeId, request.UserId);
            if (!result)
                return BadRequest("Một hoặc nhiều ghế không thể chọn vì đã bị giữ hoặc đặt trước.");

            return Ok(new { message = "Ghế đã được chọn thành công." });
        }

        [HttpPost("reserve-seats")]
        public IActionResult ReserveSeats([FromBody] SelectSeatsRequest request)
        {
            var result = _bookingService.ReserveSeats(request.SeatIds, request.ShowtimeId, request.UserId);
            if (!result)
                return BadRequest("Không thể giữ chỗ cho một hoặc nhiều ghế.");

            return Ok(new { message = "Ghế đã được giữ thành công." });
        }

        [HttpPost("release-seats")]
        public IActionResult ReleaseSeats([FromBody] SelectSeatsRequest request)
        {
            return Ok(new { message = "Ghế đã được hủy giữ thành công." });
        }

        [HttpPost("confirm-order")]
        public IActionResult ConfirmOrder([FromBody] ConfirmOrderRequest request)
        {
            var success = _bookingService.ConfirmOrder(request);
            if (!success)
                return BadRequest("Không thể xác nhận đơn hàng. Một hoặc nhiều ghế không khả dụng.");

            return Ok(new { message = "Đơn hàng đã được xác nhận thành công." });
        }
    }
}