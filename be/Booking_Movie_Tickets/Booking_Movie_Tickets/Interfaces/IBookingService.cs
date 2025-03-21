using Booking_Movie_Tickets.DTOs.Movies.Response;
using Booking_Movie_Tickets.DTOs.Seats;
using Booking_Movie_Tickets.Models.Cinemas;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<ShowtimeResponse>> GetShowtimesByMovieId(Guid movieId);
        Task<List<SeatResponse>> GetSeatsByShowtime(Guid showtimeId);
        void ReleaseSeats(SelectSeatsRequest selectSeatsRequest);
        Task<bool> ConfirmOrder(ConfirmOrderRequest request);
        bool SelectSeats(SelectSeatsRequest selectSeatsRequest);
        decimal CalculateSeatPrice(List<Guid> seatIds, Guid showtimeId);
    }
}
