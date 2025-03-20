using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.Models.Orders;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface IExtraService
    {
        Extra GetExtraById(Guid extraId);
        decimal GetPrice(Guid extraId);
        Task<PagedResult<Extra>> GetAllExtrasAsync(PagedFilterBase filter);
    }
}
