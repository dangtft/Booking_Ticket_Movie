using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Movies;
using Microsoft.EntityFrameworkCore;
using Azure.Core;
using Booking_Movie_Tickets.DTOs.Movies.Request;
using Booking_Movie_Tickets.DTOs.Others;

namespace Booking_Movie_Tickets.Services
{
    public class GenreService : IGenreService
    {
        private readonly BookingDbContext _context;
        private readonly ILogger<GenreService> _logger;

        public GenreService(BookingDbContext context, ILogger<GenreService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PagedResult<Genre>> GetAllGenresAsync(PagedFilterBase filter)
        {
            var query = _context.Genres.AsQueryable();

            //if (!string.IsNullOrEmpty(filter.Key))
            //{
            //    var keyParts = filter.Key.Split(';');

            //    foreach (var part in keyParts)
            //    {
            //        var segments = part.Split(':', 2);
            //        if (segments.Length < 2) continue;

            //        var type = segments[0].ToLower();
            //        var value = segments[1];

            //        switch (type)
            //        {
            //            case "search":
            //                query = query.Where(m => EF.Functions.Like(m.GenreName, $"%{value}%"));
            //                break;

            //            case "sort":
            //                var sortParts = value.Split(':');
            //                if (sortParts.Length == 2)
            //                {
            //                    var field = sortParts[0].ToLower();
            //                    var order = sortParts[1].ToLower();

            //                    bool isAscending = order == "asc" || order == "ascending";
            //                    bool isDescending = order == "dsc" || order == "desc" || order == "descending";

            //                    query = field switch
            //                    {
            //                        "name" => isAscending ? query.OrderBy(m => m.GenreName) :
            //                                  isDescending ? query.OrderByDescending(m => m.GenreName) : query,
            //                        _ => query
            //                    };
            //                }
            //                break;
            //        }
            //    }
            //}

            var genres = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<Genre>
            {
                Data = genres,
                Page = filter.Page,
                PageSize = filter.PageSize,
            };
        }


        public async Task<Genre> GetGenreByIdAsync(Guid id)
        {
            return await _context.Genres.FindAsync(id);
        }

        public async Task<Genre> AddGenreAsync(GenreRequest genre)
        {
            var newGenre = new Genre
            {
                Id = Guid.NewGuid(),
                GenreName = genre.GenreName
            };

            _context.Genres.Add(newGenre);
            await _context.SaveChangesAsync();
            return newGenre;
        }

        public async Task<Genre> UpdateGenreAsync(Guid id, GenreRequest genre)
        {
            var existingGenre = await _context.Genres.FindAsync(id);
            if (existingGenre == null) return null;

            existingGenre.GenreName = genre.GenreName;

            await _context.SaveChangesAsync();
            return existingGenre;
        }

        public async Task<bool> DeleteGenreAsync(Guid id)
        {
            var existing = await _context.Genres.FindAsync(id);
            if (existing == null) return false;

            _context.Genres.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
