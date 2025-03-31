namespace Booking_Movie_Tickets.DTOs.Actors.Response
{
    public class ActorDetailResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int? Age { get; set; }
        public string? Details { get; set; }
        public string? ImageURL { get; set; }
        public List<MovieRoleResponse> Movies { get; set; } = new List<MovieRoleResponse>();
    }
}
