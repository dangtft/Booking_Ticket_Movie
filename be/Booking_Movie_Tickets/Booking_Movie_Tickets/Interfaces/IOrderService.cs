using Booking_Movie_Tickets.DTOs.Orders.Request;
using Booking_Movie_Tickets.DTOs.Orders.Response;
using Booking_Movie_Tickets.Models.Orders;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface IOrderService
    {
        Task<bool> CreateOrderAsync(OrderRequest request);
        Task CleanupUnpaidOrdersAsync();
    }
}
