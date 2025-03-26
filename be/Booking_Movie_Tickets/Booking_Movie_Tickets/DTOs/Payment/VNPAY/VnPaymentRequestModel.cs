namespace Booking_Movie_Tickets.DTOs.Payment.VNPAY
{
    public class VnPaymentRequestModel
    {
        public Guid OrderId { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
