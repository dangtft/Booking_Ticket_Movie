using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.DTOs.Orders.Request;
using Booking_Movie_Tickets.DTOs.Orders.Response;
using Booking_Movie_Tickets.DTOs.Seats;
using Booking_Movie_Tickets.Helper;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Orders;
using Microsoft.EntityFrameworkCore;

namespace Booking_Movie_Tickets.Services
{
    public class OrderService : IOrderService
    {
        private readonly BookingDbContext _context;
        public OrderService(BookingDbContext context)
        {
            _context = context;
        }

        //public async Task<OrderResponse> CreateOrderAsync(HttpContext httpContext, OrderRequest request)
        //{
        //    var selectedSeats = httpContext.Session.GetObject<List<SeatSelection>>("SelectedSeats") ?? new List<SeatSelection>();

        //    if (!selectedSeats.Any())
        //    {
        //        throw new Exception("Bạn chưa chọn ghế nào để đặt vé!");
        //    }

        //    // Tính tổng tiền
        //    decimal totalAmount = 0;
        //    var orderDetails = new List<OrderDetail>();

        //    foreach (var seat in selectedSeats)
        //    {
        //        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.SeatId == seat.SeatId && t.ShowTimeId == seat.ShowtimeId);
        //        if (ticket == null) throw new Exception("Không tìm thấy thông tin vé!");

        //        totalAmount += ticket.Price;

        //        orderDetails.Add(new OrderDetail
        //        {
        //            Id = Guid.NewGuid(),
        //            TicketId = ticket.Id,
        //            Quantity = 1,
        //            Subtotal = ticket.Price
        //        });
        //    }

        //    // Tạo Order mới
        //    var order = new Order
        //    {
        //        Id = Guid.NewGuid(),
        //        UserId = request.UserId,
        //        TotalAmount = totalAmount,
        //        IsDeleted = false,
        //        OrderDetails = orderDetails
        //    };

        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();

        //    // Xóa ghế đã chọn khỏi session
        //    httpContext.Session.SetObject("SelectedSeats", new List<SeatSelection>());

        //    return new OrderResponse
        //    {
        //        OrderId = order.Id,
        //        TotalAmount = totalAmount,
        //        Message = "Đơn hàng đã được tạo, vui lòng thanh toán!"
        //    };
        //}

        public void CreateOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }
        public async Task CleanupUnpaidOrdersAsync()
        {
            var expirationTime = DateTime.UtcNow.AddMinutes(-10);

            var unpaidOrders = await _context.Orders
                .Where(o => !_context.Payments.Any(p => p.OrderId == o.Id) && o.OrderDetails.Any())
                .ToListAsync();

            foreach (var order in unpaidOrders)
            {
                order.IsDeleted = true;

                // Cập nhật trạng thái ghế thành "Available"
                foreach (var detail in order.OrderDetails)
                {
                    var seatStatus = await _context.SeatStatuses.FirstOrDefaultAsync(s => s.Seat_Id == detail.Ticket.SeatId);
                    if (seatStatus != null)
                    {
                        seatStatus.Status = "Available";
                        seatStatus.Updated_At = DateTime.UtcNow;
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
