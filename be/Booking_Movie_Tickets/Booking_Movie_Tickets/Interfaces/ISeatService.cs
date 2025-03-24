using Booking_Movie_Tickets.Models.Cinemas;
using Booking_Movie_Tickets.Models.Rooms;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface ISeatService
    {
        Task<bool> SaveSeatStatusAsync(SeatStatusTracking seatStatus);

    }
}
