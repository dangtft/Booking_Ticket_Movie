namespace Booking_Movie_Tickets.DTOs.Seats.Response
{
    public class SeatStatusResponse
    {
        public Guid SeatId { get; set; }
        public string RoomName { get; set; }
        public int Number { get; set; }
        public string SeatType { get; set; }
        public string Status { get; set; }
    }
}
