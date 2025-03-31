using Booking_Movie_Tickets.DTOs.Actors.Response;
using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.Models.Movies;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface IActorService
    {
        Task<PagedResult<Actor>> GetAllAsync(PagedFilterBase filter);
        Task<ActorDetailResponse?> GetByIdAsync(Guid actorId);
        Task<IEnumerable<ActorResponse>> GetByMovieIdAsync(Guid movieId);
        Task<Actor> CreateAsync(Actor actor);
        Task<bool> UpdateAsync(Actor actor);
        Task<bool> DeleteAsync(Guid actorId);
    }
}
