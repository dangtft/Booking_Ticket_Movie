using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.Models.Movies;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface IMovieMediaService
    {
        Task<PagedResult<MovieMedia>> GetAllMovieMediaAsync(PagedFilterBase filter);
        Task<MovieMedia?> GetMovieMediaByIdAsync(Guid id);
        Task<List<MovieMedia>> GetMovieMediaByMovieIdAsync(Guid movieId);
        Task<MovieMedia> CreateMovieMediaAsync(MovieMedia media);
        Task<MovieMedia?> UpdateMovieMediaAsync(Guid id, MovieMedia media);
        Task<bool> DeleteMovieMediaAsync(Guid id);
    }
}
