namespace Booking_Movie_Tickets.Models.Orders
{
    public class OrderDetailExtras
    {
        public Guid OrderDetailId { get; set; }
        public Guid ExtraId { get; set; }
        public int Quantity { get; set; }

        public OrderDetail OrderDetail { get; set; } = null!;
        public Extra Extra { get; set; } = null!;
    }
}
