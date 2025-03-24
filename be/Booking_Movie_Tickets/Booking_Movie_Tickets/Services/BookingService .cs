using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.DTOs.Movies.Response;
using Booking_Movie_Tickets.DTOs.Seats;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Cinemas;
using Booking_Movie_Tickets.Models.Movies;
using Booking_Movie_Tickets.Models.Orders;
using Booking_Movie_Tickets.Models.Rooms;
using Booking_Movie_Tickets.Models.Tickets;
using Microsoft.EntityFrameworkCore;

namespace Booking_Movie_Tickets.Services
{
    public class BookingService : IBookingService
    {
        private readonly BookingDbContext _context;
        private readonly IOrderService _orderService;
        private readonly IExtraService _extraService;

        public BookingService(BookingDbContext context, IOrderService orderService, IExtraService extraService)
        {
            _context = context;
            _orderService = orderService;
            _extraService = extraService;
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

            var seatStatuses = await _context.SeatStatuses
                .Where(s => s.Show_Time_Id == showtimeId)
                .ToDictionaryAsync(s => s.Seat_Id);

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

        public async Task<bool> LockedSeat(Guid seatId, string userId)
        {
            var seatStatus = await _context.SeatStatuses.FirstOrDefaultAsync(s => s.Seat_Id == seatId);

            if (seatStatus == null)
            {
                seatStatus = new SeatStatusTracking
                {
                    Id = Guid.NewGuid(),
                    Seat_Id = seatId,
                    Status = "Locked",
                    IsLocked = true,
                    LockedByUserId = userId,
                    LockedAt = DateTime.UtcNow
                };

                await _context.SeatStatuses.AddAsync(seatStatus);
            }
            else
            {
                if (seatStatus.IsLocked)
                    return false;

                seatStatus.Status = "Locked";
                seatStatus.IsLocked = true;
                seatStatus.LockedByUserId = userId;
                seatStatus.LockedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnlockSeat(Guid seatId, string userId, Guid showtimeId)
        {
            var seatStatus = await _context.SeatStatuses
                .FirstOrDefaultAsync(s => s.Seat_Id == seatId && s.Show_Time_Id == showtimeId);

            if (seatStatus == null || seatStatus.LockedByUserId != userId)
                return false; 

            _context.SeatStatuses.Remove(seatStatus);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
