using Booking_Movie_Tickets.DTOs.Movies.Response;
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

        [HttpPost("select-seats")]
        public IActionResult SelectSeats([FromBody] SelectSeatsRequest request)
        {
            var result = _bookingService.SelectSeats(request);
            if (!result)
                return BadRequest(ApiMessages.ERROR);

            return Ok(ApiMessages.SUCCESS);
        }

        [HttpPost("release-seats")]
        public IActionResult ReleaseSeats([FromBody] SelectSeatsRequest request)
        {
            try
            {
                _bookingService.ReleaseSeats(request);
                return Ok(ApiMessages.SUCCESS);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiMessages.ERROR);
            }
        }

        [HttpPost("confirm-order")]
        public async Task<IActionResult> ConfirmOrder([FromBody] ConfirmOrderRequest request)
        {
            var success = await _bookingService.ConfirmOrder(request);
            if (!success)
                return BadRequest(ApiMessages.ERROR);

            return Ok(ApiMessages.SUCCESS);
        }
    }
}