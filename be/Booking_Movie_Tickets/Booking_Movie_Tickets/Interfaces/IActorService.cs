using Booking_Movie_Tickets.Models.Movies;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface IActorService
    {
        Task<IEnumerable<Actor>> GetAllAsync();
        Task<Actor?> GetByIdAsync(Guid actorId);
        Task<IEnumerable<Actor>> GetByMovieIdAsync(Guid movieId);
        Task<Actor> CreateAsync(Actor actor);
        Task<bool> UpdateAsync(Actor actor);
        Task<bool> DeleteAsync(Guid actorId);
    }
}
