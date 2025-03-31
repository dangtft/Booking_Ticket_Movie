namespace Booking_Movie_Tickets.DTOs.Seats.Response
{
    public class SeatResponse
    {
        public Guid SeatId { get; set; }
        public string Row { get; set; }
        public int Number { get; set; }
        public string Type { get; set; }
        public decimal PriceModifier { get; set; }
        public string RoomName { get; set; }
    }
}
