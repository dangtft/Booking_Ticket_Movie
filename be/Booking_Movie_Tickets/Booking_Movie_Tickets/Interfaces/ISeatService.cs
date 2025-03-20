using Booking_Movie_Tickets.Models.Cinemas;
using Booking_Movie_Tickets.Models.Rooms;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface ISeatService
    {
        List<Seat> GetAvailableSeats(Guid showtimeId);
        SeatStatusTracking GetSeatStatus(Guid seatId, Guid showtimeId);
        List<SeatStatusTracking> GetLockedSeats();
        void UpdateSeatStatus(SeatStatusTracking seat);

    }
}
