using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.DTOs.Seats.Response;
using Booking_Movie_Tickets.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Booking_Movie_Tickets.Services
{
    public class SeatService : ISeatService
    {
        private readonly BookingDbContext _context;

        public SeatService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<SeatResponse>> GetSeatsByRoomId(PagedFilterBase filter, Guid roomId)
        {
            var seatQuery = _context.Seats
                .Where(seat => seat.RoomId == roomId)
                .Include(seat => seat.SeatType)
                .Include(seat => seat.Room)
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

            var totalCount = await seatQuery.CountAsync();

            var seats = await seatQuery
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<SeatResponse>
            {
                Data = seats,
                Page = filter.Page,
                TotalCount = totalCount,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
            };
        }
    }
}
