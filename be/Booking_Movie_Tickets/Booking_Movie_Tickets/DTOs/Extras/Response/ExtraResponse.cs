namespace Booking_Movie_Tickets.DTOs.Extras.Response
{
    public class ExtraResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
        public string? ImageURL { get; set; }
    }
}
