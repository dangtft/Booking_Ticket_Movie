namespace Booking_Movie_Tickets.DTOs.Seats
{
    public class SeatSelection
    {
        public Guid SeatId { get; set; }
        public Guid ShowtimeId { get; set; }
        public DateTime SelectedAt { get; set; }
    }
}
