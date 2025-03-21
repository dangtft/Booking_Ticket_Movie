using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.DTOs.Movies.Response;
using Booking_Movie_Tickets.DTOs.Seats;
using Booking_Movie_Tickets.Interfaces;
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
        public bool SelectSeats(SelectSeatsRequest selectSeatsRequest)
        {
            var seatStatuses = _context.SeatStatuses
                .Where(s => selectSeatsRequest.SeatIds.Contains(s.Seat_Id) && s.Show_Time_Id == selectSeatsRequest.ShowtimeId)
                .ToList();

            foreach (var seatId in selectSeatsRequest.SeatIds)
            {
                var seatStatus = seatStatuses.FirstOrDefault(s => s.Seat_Id == seatId);

                if (seatStatus == null)
                {
                    seatStatus = new SeatStatusTracking
                    {
                        Id = Guid.NewGuid(),
                        Seat_Id = seatId,
                        Room_Id = selectSeatsRequest.RoomId,
                        Show_Time_Id = selectSeatsRequest.ShowtimeId,
                        Status = "Reserved",
                        IsLocked = true,
                        LockedAt = DateTime.UtcNow,
                        LockedByUserId = selectSeatsRequest.UserId ?? "anonymous",
                        Updated_At = DateTime.UtcNow
                    };
                    _context.SeatStatuses.Add(seatStatus);
                }
                else if (seatStatus.IsLocked && seatStatus.LockedByUserId != selectSeatsRequest.UserId)
                {
                    return false;
                }
                else
                {
                    seatStatus.IsLocked = true;
                    seatStatus.LockedAt = DateTime.UtcNow;
                    seatStatus.LockedByUserId = selectSeatsRequest.UserId ?? "anonymous";
                    seatStatus.Status = "Reserved";
                    seatStatus.Updated_At = DateTime.UtcNow;
                }
            }
            _context.SaveChanges();
            return true;
        }

        public void ReleaseSeats(SelectSeatsRequest selectSeatsRequest)
        {
            var seatStatuses = _context.SeatStatuses
                .Where(s => selectSeatsRequest.SeatIds.Contains(s.Seat_Id) && s.Show_Time_Id == selectSeatsRequest.ShowtimeId)
                .ToList();

            _context.SeatStatuses.RemoveRange(seatStatuses);
            _context.SaveChanges();
        }

        public async Task<bool> ConfirmOrder(ConfirmOrderRequest request)
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
                    QRCode = Guid.NewGuid().ToString(),
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
                        Subtotal = (decimal)extra.Quantity * await _extraService.GetPriceAsync(extra.ExtraId)
                    });
                }
            }

            _orderService.CreateOrder(newOrder);
            return true;
        }

        public decimal CalculateSeatPrice(List<Guid> seatIds, Guid showtimeId)
        {
            var showtime = _context.Showtimes.FirstOrDefault(s => s.Id == showtimeId);
            if (showtime == null) throw new Exception("Không tìm thấy suất chiếu.");

            var seats = _context.Seats
                .Include(s => s.SeatType)
                .Where(s => seatIds.Contains(s.Id))
                .ToList();

            if (!seats.Any()) throw new Exception("Không tìm thấy ghế.");

            return seats.Sum(s => showtime.Price * s.SeatType.PriceModifier);
        }

    }
}
