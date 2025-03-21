namespace Booking_Movie_Tickets.DTOs.Authentication.Response
{
    public class RegistrationResponse
    {
        public bool IsSuccess { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
