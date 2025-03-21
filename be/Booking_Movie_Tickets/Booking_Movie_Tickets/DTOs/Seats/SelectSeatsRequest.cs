namespace Booking_Movie_Tickets.DTOs.Seats
{
    public class SelectSeatsRequest
    {
        public List<Guid> SeatIds { get; set; }
        public Guid RoomId {  get; set; }
        public Guid ShowtimeId { get; set; }
        public string? UserId { get; set; } = "anonymous";
    }
}
