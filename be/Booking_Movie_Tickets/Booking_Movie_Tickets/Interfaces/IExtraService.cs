using Booking_Movie_Tickets.DTOs.Extras.Request;
using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.Models.Orders;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface IExtraService
    {
        Task<Extra> GetExtraByIdAsync(Guid extraId);
        Task<decimal> GetPriceAsync(Guid extraId);
        Task<PagedResult<Extra>> GetAllExtrasAsync(PagedFilterBase filter);
        Task<Extra> CreateExtraAsync(ExtraRequest request);
        Task<bool> UpdateExtraAsync(Guid extraId, ExtraRequest request);
        Task<bool> DeleteExtraAsync(Guid extraId);
    }
}
