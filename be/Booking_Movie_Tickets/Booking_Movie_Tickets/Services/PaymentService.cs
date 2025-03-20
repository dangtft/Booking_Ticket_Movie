using Booking_Movie_Tickets.DTOs.Payment.Request;
using Booking_Movie_Tickets.DTOs.Payment.Response;
using Booking_Movie_Tickets.Models.Payments;
using Microsoft.EntityFrameworkCore;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Data;

namespace Booking_Movie_Tickets.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly BookingDbContext _context;
        public PaymentService(BookingDbContext context)
        {
            _context = context;
        }

        public bool ProcessPayment(string userId, decimal totalPrice)
        {
            throw new NotImplementedException();
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
        {
            // Kiểm tra đơn hàng
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId);

            if (order == null) throw new Exception("Đơn hàng không tồn tại!");

            // Giả lập xử lý thanh toán (có thể thay thế bằng VNPay, Momo, Stripe...)
            var transactionId = Guid.NewGuid();

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                OrderId = request.OrderId,
                Amount = order.TotalAmount,
                TransactionId = transactionId,
                PaymentMethodId = request.PaymentMethodId,
                //PaymentStatusId = "Success",
                IsDeleted = false
            };

            _context.Payments.Add(payment);

            // Cập nhật trạng thái ghế thành "Booked"
            foreach (var detail in order.OrderDetails)
            {
                var seatStatus = await _context.SeatStatuses.FirstOrDefaultAsync(s => s.Seat_Id == detail.Ticket.SeatId);
                if (seatStatus != null)
                {
                    seatStatus.Status = "Booked";
                    seatStatus.Updated_At = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();

            return new PaymentResponse
            {
                TransactionId = transactionId.ToString(),
                Status = "Success",
                Message = "Thanh toán thành công, vé của bạn đã được xác nhận!"
            };
        }

    }
}
