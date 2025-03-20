using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.DTOs.Movies.Response;
using Booking_Movie_Tickets.DTOs.Seats;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Cinemas;
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
                .Where(s => s.MovieId == movieId && s.StartTime > DateTime.UtcNow)
                .Select(s => new ShowtimeResponse
                {
                    Id = s.Id,
                    MovieId = s.MovieId,
                    StartTime = s.StartTime,
                    RoomId = s.RoomId,
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
                Number = seat.SeatNumber,
                Type = seat.SeatType.TypeName,
                RoomName = seat.Room.Name,
                Status = seatStatuses.ContainsKey(seat.Id) ? seatStatuses[seat.Id].Status : "Available",
                IsLocked = seatStatuses.ContainsKey(seat.Id) ? seatStatuses[seat.Id].IsLocked : false,
                LockedByUserId = seatStatuses.ContainsKey(seat.Id) ? seatStatuses[seat.Id].LockedByUserId : null,
                LockedAt = seatStatuses.ContainsKey(seat.Id) ? seatStatuses[seat.Id].LockedAt : null
            }).ToList();

            return seats;
        }

        // Người dùng chọn ghế -> lưu vào SeatStatuses
        public bool SelectSeats(List<Guid> seatIds, Guid showtimeId, string? userId = "anonymous")
        {
            var seatStatuses = _context.SeatStatuses
                .Where(s => seatIds.Contains(s.Seat_Id) && s.Show_Time_Id == showtimeId)
                .ToList();

            foreach (var seatId in seatIds)
            {
                var seatStatus = seatStatuses.FirstOrDefault(s => s.Seat_Id == seatId);

                if (seatStatus == null)
                {
                    seatStatus = new SeatStatusTracking
                    {
                        Id = Guid.NewGuid(),
                        Seat_Id = seatId,
                        Show_Time_Id = showtimeId,
                        Status = "Reserved",
                        IsLocked = true,
                        LockedAt = DateTime.UtcNow,
                        LockedByUserId = userId ?? "anonymous",
                        Updated_At = DateTime.UtcNow
                    };
                    _context.SeatStatuses.Add(seatStatus);
                }
                else if (seatStatus.IsLocked && seatStatus.LockedByUserId != userId)
                {
                    return false;
                }
                else
                {
                    seatStatus.IsLocked = true;
                    seatStatus.LockedAt = DateTime.UtcNow;
                    seatStatus.LockedByUserId = userId ?? "anonymous";
                    seatStatus.Status = "Reserved";
                    seatStatus.Updated_At = DateTime.UtcNow;
                }
            }
            _context.SaveChanges();
            return true;
        }

        public bool ReserveSeats(List<Guid> seatIds, Guid showtimeId, string? userId = "anonymous")
        {
            return SelectSeats(seatIds, showtimeId, userId);
        }

        public void ReleaseSeats(List<Guid> seatIds, Guid showtimeId)
        {
            var seatStatuses = _context.SeatStatuses
                .Where(s => seatIds.Contains(s.Seat_Id) && s.Show_Time_Id == showtimeId)
                .ToList();

            _context.SeatStatuses.RemoveRange(seatStatuses);
            _context.SaveChanges();
        }

        public bool ConfirmOrder(ConfirmOrderRequest request)
        {
            var selectedSeats = _context.SeatStatuses
                .Where(s => request.SelectedSeats.Contains(s.Seat_Id) && s.Show_Time_Id == request.ShowtimeId && s.Status == "Reserved")
                .ToList();

            if (selectedSeats.Count != request.SelectedSeats.Count)
                return false;

            var newOrder = new Order
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId ?? "anonymous",
                TotalAmount = request.TotalPrice,
                IsDeleted = false,
                OrderDetails = new List<OrderDetail>()
            };

            foreach (var seat in selectedSeats)
            {
                var ticket = new Ticket
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId ?? "anonymous",
                    ShowTimeId = request.ShowtimeId,
                    BookingCode = Guid.NewGuid().ToString("N").Substring(0, 8),
                    SeatId = seat.Seat_Id,
                    TicketTypeId = request.TicketTypeId,
                    TicketStatusId = request.TicketStatusId,
                    TicketPrice = request.TicketPrice,
                    CreatedAt = DateTime.UtcNow
                };

                newOrder.OrderDetails.Add(new OrderDetail
                {
                    Id = Guid.NewGuid(),
                    OrderId = newOrder.Id,
                    TicketId = ticket.Id,
                    Quantity = 1,
                    Subtotal = ticket.TicketPrice
                });

                seat.Status = "Booked";
                seat.IsLocked = false;
                seat.LockedByUserId = null;
                seat.Updated_At = DateTime.UtcNow;
                _context.Tickets.Add(ticket);
            }

            if (request.Extras != null)
            {
                foreach (var extra in request.Extras)
                {
                    newOrder.OrderDetails.Add(new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        OrderId = newOrder.Id,
                        ExtraId = extra.ExtraId,
                        Quantity = extra.Quantity,
                        Subtotal = extra.Quantity * _extraService.GetPrice(extra.ExtraId)
                    });
                }
            }

            _orderService.CreateOrder(newOrder);
            return true;
        }

    }
}
