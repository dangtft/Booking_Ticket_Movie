using Booking_Movie_Tickets.Models.Movies;
using Booking_Movie_Tickets.DTOs.Movies.Request;
using Booking_Movie_Tickets.DTOs.Others;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface IGenreService
    {
        Task<PagedResult<Genre>> GetAllGenresAsync(PagedFilterBase filter);
        Task<Genre> GetGenreByIdAsync(Guid id);
        Task<Genre> AddGenreAsync(GenreRequest genre);
        Task<Genre> UpdateGenreAsync(Guid id, GenreRequest genre);
        Task<bool> DeleteGenreAsync(Guid id);
    }
}
