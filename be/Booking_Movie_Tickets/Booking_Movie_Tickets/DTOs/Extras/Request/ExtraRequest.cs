namespace Booking_Movie_Tickets.DTOs.Extras.Request
{
    public class ExtraRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsDeleted { get; set; }
        public int Quantity { get; set; }
    }
}
