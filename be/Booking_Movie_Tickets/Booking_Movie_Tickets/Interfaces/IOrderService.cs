using Booking_Movie_Tickets.DTOs.Orders.Request;
using Booking_Movie_Tickets.DTOs.Orders.Response;
using Booking_Movie_Tickets.DTOs.Tickets.Response;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(OrderRequest request);
        Task<List<OrderResponse>> GetOrdersByUserIdAsync(string userId);
        Task<List<TicketResponse>> GetTicketsByOrderIdAsync(Guid orderId);
        Task<bool> DeleteOrderById(Guid orderId);
        Task<bool> UpdateOrderStatus(Guid orderId, string status);
        Task<string> GetOrderStatus(Guid orderId);
    }
}
