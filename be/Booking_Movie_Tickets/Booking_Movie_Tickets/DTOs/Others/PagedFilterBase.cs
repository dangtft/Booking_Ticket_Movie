namespace Booking_Movie_Tickets.DTOs.Others
{
    public class PagedFilterBase
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public string? Search { get; set; }
        public string? Sort { get; set; }
    }
}
