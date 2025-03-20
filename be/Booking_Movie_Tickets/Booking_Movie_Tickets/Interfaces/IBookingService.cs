using Booking_Movie_Tickets.DTOs.Movies.Response;
using Booking_Movie_Tickets.DTOs.Seats;
using Booking_Movie_Tickets.Models.Cinemas;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<ShowtimeResponse>> GetShowtimesByMovieId(Guid movieId);
        bool ReserveSeats(List<Guid> seatIds, Guid showtimeId, string? userId = "anonymous");
        Task<List<SeatResponse>> GetSeatsByShowtime(Guid showtimeId);
        void ReleaseSeats(List<Guid> seatIds, Guid showtimeId);
        bool ConfirmOrder(ConfirmOrderRequest request);
        bool SelectSeats(List<Guid> seatIds, Guid showtimeId, string? userId = "anonymous");
    }
}
