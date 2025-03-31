using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Movies;
using Microsoft.EntityFrameworkCore;
using Booking_Movie_Tickets.DTOs.Actors.Response;
using Booking_Movie_Tickets.DTOs.Others;

namespace Booking_Movie_Tickets.Services
{
    public class ActorService : IActorService
    {
        private readonly BookingDbContext _context;

        public ActorService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Actor>> GetAllAsync(PagedFilterBase filter)
        {
            var query = _context.Actors.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(a => a.Name.Contains(filter.Search));
            }

            switch (filter.Sort?.ToLower())
            {
                case "name_asc":
                    query = query.OrderBy(a => a.Name);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(a => a.Name);
                    break;
                case "age_asc":
                    query = query.OrderBy(a => a.Age);
                    break;
                case "age_desc":
                    query = query.OrderByDescending(a => a.Age);
                    break;
                default:
                    query = query.OrderBy(a => a.Name); 
                    break;
            }

            var totalCount = await query.CountAsync();

            var actors = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<Actor>
            {
                Data = actors,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
            };
        }

        public async Task<ActorDetailResponse?> GetByIdAsync(Guid actorId)
        {
            var actor = await _context.Actors
                .Include(a => a.MovieActors)
                .ThenInclude(ma => ma.Movie)
                .FirstOrDefaultAsync(a => a.Id == actorId);

            if (actor == null) return null;

            return new ActorDetailResponse
            {
                Id = actor.Id,
                Name = actor.Name,
                Age = actor.Age,
                Details = actor.Details,
                ImageURL = actor.ImageURL,
                Movies = actor.MovieActors.Select(ma => new MovieRoleResponse
                {
                    MovieName = ma.Movie.Title,
                    Role = ma.Role
                }).ToList()
            };
        }

        public async Task<IEnumerable<ActorResponse>> GetByMovieIdAsync(Guid movieId)
        {
            return await _context.MovieActors
                .Where(ma => ma.MovieId == movieId)
                .Select(ma => new ActorResponse
                {
                    Id = ma.Actor.Id,
                    Name = ma.Actor.Name,
                    ImageURL = ma.Actor.ImageURL,
                    Role = ma.Role
                })
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
