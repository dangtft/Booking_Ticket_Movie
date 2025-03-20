using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.DTOs.Movies.Response
{
    public class MediaResponse
    {
        public string Description { get; set; }

        public string MediaType { get; set; }

        public string MediaURL { get; set; }
    }
}
