namespace Booking_Movie_Tickets.DTOs.Payment.VNPAY
{
    public class PaymentReturnDtos
    {
        public string? PaymentId { get; set; }
        /// <summary>
        /// 00: Success
        /// 99: Unknown
        /// 10: Error
        /// </summary>
        public string? PaymentStatus { get; set; }
        public string? PaymentMessage { get; set; }
        /// <summary>
        /// Format: yyyyMMddHHmmss
        /// </summary>
        public string? PaymentDate { get; set; }
        public string? PaymentRefId { get; set; }
        public decimal? Amount { get; set; }
        public string? Signature { get; set; }

    }
}
