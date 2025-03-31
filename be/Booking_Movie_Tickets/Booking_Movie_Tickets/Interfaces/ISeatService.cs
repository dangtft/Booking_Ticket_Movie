using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.DTOs.Seats.Response;
using Booking_Movie_Tickets.Models.Rooms;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface ISeatService
    {
        Task<PagedResult<SeatResponse>> GetSeatsByRoomId(PagedFilterBase filter, Guid roomId);
    }
}
