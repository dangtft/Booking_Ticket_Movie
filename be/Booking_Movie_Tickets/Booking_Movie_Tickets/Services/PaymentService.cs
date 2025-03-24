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

        public Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
