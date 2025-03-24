using Booking_Movie_Tickets.DTOs.Movies.Response;
using Booking_Movie_Tickets.DTOs.Seats;
using Booking_Movie_Tickets.Models.Cinemas;
using Booking_Movie_Tickets.Models.Movies;
using Booking_Movie_Tickets.Models.Orders;
using Booking_Movie_Tickets.Models.Tickets;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<ShowtimeResponse>> GetShowtimesByMovieId(Guid movieId);
        Task<List<SeatResponse>> GetSeatsByShowtime(Guid showtimeId);
        Task<bool> LockedSeat(Guid seatId, string userId);
    }
}
