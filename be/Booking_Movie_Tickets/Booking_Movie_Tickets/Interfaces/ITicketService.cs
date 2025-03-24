using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.DTOs.Tickets.Request;
using Booking_Movie_Tickets.Models.Tickets;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface ITicketService
    {
        Ticket CreateTicket(TicketRequest request);
        Task<string?> GetQRCodeAsync(Guid ticketId);
        Task<PagedResult<TicketType>> GetAllTicketType(PagedFilterBase filter);
    }
}
