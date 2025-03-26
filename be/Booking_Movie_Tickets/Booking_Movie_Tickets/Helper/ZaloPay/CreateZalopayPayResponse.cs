namespace Booking_Movie_Tickets.Helper.ZaloPay
{
    public class CreateZalopayPayResponse
    {
        public int returnCode { get; set; }
        public string returnMessage { get; set; } = string.Empty;
        public string orderUrl { get; set; } = string.Empty;
    }
}
