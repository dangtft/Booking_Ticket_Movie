using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.DTOs.Seats.Request;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Rooms;

namespace Booking_Movie_Tickets.Services
{
    public class SeatService : ISeatService
    {
        private readonly BookingDbContext _context;

        public SeatService(BookingDbContext context)
        {
            _context = context;
        }
    }
}
