using Booking_Movie_Tickets.DTOs.Payment.VNPAY;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface IVnPayService
    {
        string CreatePaymentURL(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExcute(IQueryCollection collections);
    }
}
