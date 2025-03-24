using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Cinemas;
using Booking_Movie_Tickets.Models.Rooms;
using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> SaveSeatStatusAsync(SeatStatusTracking seatStatus)
        {
            var existingSeatStatus = await _context.SeatStatuses
                .FirstOrDefaultAsync(s => s.Seat_Id == seatStatus.Seat_Id && s.Show_Time_Id == seatStatus.Show_Time_Id);

            if (existingSeatStatus != null)
            {
                existingSeatStatus.Status = seatStatus.Status;
                existingSeatStatus.IsLocked = seatStatus.IsLocked;
                existingSeatStatus.LockedByUserId = seatStatus.LockedByUserId;
                existingSeatStatus.LockedAt = seatStatus.LockedAt;
                existingSeatStatus.ExpirationTime = seatStatus.ExpirationTime;
                existingSeatStatus.Updated_At = DateTime.UtcNow;
            }
            else
            {
                seatStatus.Id = Guid.NewGuid();
                seatStatus.Updated_At = DateTime.UtcNow;
                await _context.SeatStatuses.AddAsync(seatStatus);
            }

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
