using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.DTOs.Movies.Response;
using Booking_Movie_Tickets.DTOs.Others;
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
        public async Task<PagedResult<ShowtimeResponse>> GetShowtimesByMovieId(PagedFilterBase filter, Guid movieId)
        {
            var query = _context.Showtimes
                .Where(s => s.MovieId == movieId)
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
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Sort))
            {
                switch (filter.Sort.ToLower())
                {
                    case "date_asc":
                        query = query.OrderBy(s => s.Date).ThenBy(s => s.Time);
                        break;
                    case "date_desc":
                        query = query.OrderByDescending(s => s.Date).ThenByDescending(s => s.Time);
                        break;
                    default:
                        query = query.OrderBy(s => s.Date).ThenBy(s => s.Time);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(s => s.Date).ThenBy(s => s.Time);
            }

            var totalCount = await query.CountAsync();

            var showtimes = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<ShowtimeResponse>
            {
                Data = showtimes,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
            };
        }

        public async Task<PagedResult<SeatResponse>> GetSeatsByShowtime(PagedFilterBase filter, Guid showtimeId)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.Room)
                .ThenInclude(r => r.Seats)
                .ThenInclude(seat => seat.SeatType)
                .FirstOrDefaultAsync(s => s.Id == showtimeId);

            if (showtime == null)
            {
                return new PagedResult<SeatResponse>
                {
                    Data = new List<SeatResponse>(),
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    TotalCount = 0,
                    TotalPages = 0
                };
            }

            var bookedSeats = await _context.Tickets
                .Where(t => t.ShowTimeId == showtimeId)
                .Select(t => t.SeatId)
                .ToHashSetAsync();

            var seatQuery = showtime.Room.Seats
                .Where(seat => !bookedSeats.Contains(seat.Id))
                .Select(seat => new SeatResponse
                {
                    SeatId = seat.Id,
                    Row = seat.Row,
                    Number = seat.SeatNumber,
                    Type = seat.SeatType.TypeName,
                    PriceModifier = seat.SeatType.PriceModifier,
                    RoomName = seat.Room.Name
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Sort))
            {
                switch (filter.Sort.ToLower())
                {
                    case "row_asc":
                        seatQuery = seatQuery.OrderBy(s => s.Row).ThenBy(s => s.Number);
                        break;
                    case "row_desc":
                        seatQuery = seatQuery.OrderByDescending(s => s.Row).ThenByDescending(s => s.Number);
                        break;
                }
            }
            else
            {
                seatQuery = seatQuery.OrderBy(s => s.Row).ThenBy(s => s.Number);
            }

            var totalCount = seatQuery.Count();

            var seats = seatQuery
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return new PagedResult<SeatResponse>
            {
                Data = seats,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
            };
        }

    }
}
