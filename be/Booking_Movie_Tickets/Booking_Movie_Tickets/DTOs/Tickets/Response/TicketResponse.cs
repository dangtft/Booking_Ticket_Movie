using System.ComponentModel.DataAnnotations.Schema;

namespace Booking_Movie_Tickets.DTOs.Tickets.Response
{
    public class TicketResponse
    {
        public Guid Id { get; set; }

        public string? UserId { get; set; }

        public Guid ShowTimeId { get; set; }

        public string QRCode { get; set; }

        public Guid SeatId { get; set; }

        public Guid TicketTypeId { get; set; }

        public Guid TicketStatusId { get; set; }
        public decimal TicketPrice { get; set; }
    }
}
