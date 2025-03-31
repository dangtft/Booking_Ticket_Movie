using Booking_Movie_Tickets.DTOs.Movies.Response;
using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.DTOs.Seats.Response;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface IBookingService
    {
        Task<PagedResult<ShowtimeResponse>> GetShowtimesByMovieId(PagedFilterBase filter, Guid movieId);
        Task<PagedResult<SeatResponse>> GetSeatsByShowtime(PagedFilterBase filter, Guid showtimeId);
    }
}
