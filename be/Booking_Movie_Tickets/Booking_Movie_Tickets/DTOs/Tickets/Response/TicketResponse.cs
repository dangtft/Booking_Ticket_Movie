using System.ComponentModel.DataAnnotations.Schema;

namespace Booking_Movie_Tickets.DTOs.Tickets.Response
{
    public class TicketResponse
    {
        public Guid Id { get; set; }

        public DateOnly ShowTimeDate { get; set; }
        public TimeOnly ShowTimeStart { get; set; }
        public string SeatRow { get; set; }
        public int SeatNumber { get; set; }

        public string QRCode { get; set; }

        public decimal TicketPrice { get; set; }
    }
}
