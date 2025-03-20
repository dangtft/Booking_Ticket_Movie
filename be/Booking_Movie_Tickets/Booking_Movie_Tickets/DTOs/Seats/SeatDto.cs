namespace Booking_Movie_Tickets.DTOs.Seats
{
    public class SeatDto
    {
        public Guid Id { get; set; }
        public int SeatNumber { get; set; }
        public string SeatType { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
