using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Movies;
using Microsoft.EntityFrameworkCore;

namespace Booking_Movie_Tickets.Services
{
    public class ActorService : IActorService
    {
        private readonly BookingDbContext _context;

        public ActorService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Actor>> GetAllAsync()
        {
            return await _context.Actors.ToListAsync();
        }

        public async Task<Actor?> GetByIdAsync(Guid actorId)
        {
            return await _context.Actors
                .Include(a => a.MovieActors)
                .ThenInclude(ma => ma.Movie)
                .FirstOrDefaultAsync(a => a.Id == actorId);
        }

        public async Task<IEnumerable<Actor>> GetByMovieIdAsync(Guid movieId)
        {
            return await _context.MovieActors
                .Where(ma => ma.MovieId == movieId)
                .Select(ma => ma.Actor)
                .ToListAsync();
        }

        public async Task<Actor> CreateAsync(Actor actor)
        {
            _context.Actors.Add(actor);
            await _context.SaveChangesAsync();
            return actor;
        }

        public async Task<bool> UpdateAsync(Actor actor)
        {
            var existingActor = await _context.Actors.FindAsync(actor.Id);
            if (existingActor == null) return false;

            existingActor.Name = actor.Name;
            existingActor.Age = actor.Age;
            existingActor.Details = actor.Details;
            existingActor.ImageURL = actor.ImageURL;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid actorId)
        {
            var actor = await _context.Actors.FindAsync(actorId);
            if (actor == null) return false;

            _context.Actors.Remove(actor);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
