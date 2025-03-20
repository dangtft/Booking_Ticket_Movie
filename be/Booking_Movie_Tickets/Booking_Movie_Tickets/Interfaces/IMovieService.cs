using Booking_Movie_Tickets.Models.Movies;
using Booking_Movie_Tickets.DTOs.Views;
using Booking_Movie_Tickets.DTOs.Movies.Request;
using Booking_Movie_Tickets.DTOs.Movies.Response;
using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.Models.Cinemas;
using Booking_Movie_Tickets.DTOs.Seats;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface IMovieService
    {
        Task<PagedResult<MoviesResponse>> GetMoviesAsync(MovieFilter movieFilter);
        Task<Movie> AddMovieAsync(MovieRequest request);
        Task<Movie> UpdateMovieAsync(Guid id, MovieRequest request);
        Task<bool> DeleteMovieAsync(Guid id);
        Task<List<Room>> GetRoomsByMovieAndTimeAsync(Guid movieId, DateTime date, TimeSpan time);
        Task<MovieDetailResponse> GetMovieById(Guid movieId);

    }

}
