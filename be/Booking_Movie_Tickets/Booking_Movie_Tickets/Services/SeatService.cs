using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Cinemas;
using Booking_Movie_Tickets.Models.Rooms;
using System;

namespace Booking_Movie_Tickets.Services
{
    public class SeatService : ISeatService
    {
        private readonly BookingDbContext _context;

        public SeatService(BookingDbContext context)
        {
            _context = context;
        }

        public List<Seat> GetAvailableSeats(Guid showtimeId)
        {
            return _context.Seats.ToList();
        }

        public SeatStatusTracking GetSeatStatus(Guid seatId, Guid showtimeId)
        {
            return _context.SeatStatuses
                .FirstOrDefault(s => s.Seat_Id == seatId && s.Show_Time_Id == showtimeId);
        }
        public List<SeatStatusTracking> GetLockedSeats()
        {
            return _context.SeatStatuses
                           .Where(s => s.IsLocked && s.LockedAt.HasValue)
                           .ToList();
        }
        public void UpdateSeatStatus(SeatStatusTracking seat)
        {
            _context.SeatStatuses.Update(seat);
            _context.SaveChanges();
        }
    }
}
