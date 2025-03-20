using Booking_Movie_Tickets.DTOs.Payment.Request;
using Booking_Movie_Tickets.DTOs.Payment.Response;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface IPaymentService
    {
        bool ProcessPayment(string userId, decimal totalPrice);
        Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request);
    }
}
