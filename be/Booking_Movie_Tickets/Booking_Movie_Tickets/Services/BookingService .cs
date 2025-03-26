using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.DTOs.Movies.Response;
using Booking_Movie_Tickets.DTOs.Seats.Response;
using Booking_Movie_Tickets.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Booking_Movie_Tickets.Services
{
    public class BookingService : IBookingService
    {
        private readonly BookingDbContext _context;

        public BookingService(BookingDbContext context)
        {
            _context = context;
        }

        //Lấy phim và suất chiếu theo Id
        public async Task<IEnumerable<ShowtimeResponse>> GetShowtimesByMovieId(Guid movieId)
        {
            return await _context.Showtimes
                .Where(s => s.MovieId == movieId) //&& s.StartTime > DateTime.UtcNow
                .Select(s => new ShowtimeResponse
                {
                    Id = s.Id,
                    MovieId = s.MovieId,
                    Date = s.Date,
                    Time = s.Time,
                    RoomId = s.RoomId,
                    RoomName = s.Room.Name,
                    Price = s.Price,
                })
                .ToListAsync();
        }
        public async Task<List<SeatResponse>> GetSeatsByShowtime(Guid showtimeId)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.Room)
                .ThenInclude(r => r.Seats)
                .ThenInclude(seat => seat.SeatType)
                .FirstOrDefaultAsync(s => s.Id == showtimeId);

            if (showtime == null)
                return new List<SeatResponse>();

            var seatStatuses = await _context.SeatStatusTracking
                .Where(s => s.ShowTimeId == showtimeId)
                .ToDictionaryAsync(s => s.SeatId);

            var seats = showtime.Room.Seats.Select(seat => new SeatResponse
            {
                SeatId = seat.Id,
                RoomId = seat.RoomId,
                Row = seat.Row,
                Number = seat.SeatNumber,
                Type = seat.SeatType.TypeName,
                PriceModifier = seat.SeatType.PriceModifier,
                RoomName = seat.Room.Name,
                Status = seatStatuses.ContainsKey(seat.Id) ? seatStatuses[seat.Id].Status : "Available",
                IsLocked = seatStatuses.ContainsKey(seat.Id) ? seatStatuses[seat.Id].IsLocked : false,
                LockedByUserId = seatStatuses.ContainsKey(seat.Id) ? seatStatuses[seat.Id].LockedByUserId : null,
                LockedAt = seatStatuses.ContainsKey(seat.Id) ? seatStatuses[seat.Id].LockedAt : null
            }).ToList();

            return seats;
        }

    }
}
