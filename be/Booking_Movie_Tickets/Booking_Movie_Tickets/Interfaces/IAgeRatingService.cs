using Booking_Movie_Tickets.Models.Movies;
using Booking_Movie_Tickets.DTOs.Movies.Request;
using Booking_Movie_Tickets.DTOs.Others;

namespace Booking_Movie_Tickets.Interfaces
{
    public interface IAgeRatingService
    {
        Task<PagedResult<AgeRating>> GetAllAgeRatingsAsync(AgeRatingFilter filter);
        Task<AgeRating> GetAgeRatingByIdAsync(Guid id);
        Task<AgeRating> AddAgeRatingAsync(AgeRatingRequest ageRating);
        Task<AgeRating> UpdateAgeRatingAsync(Guid id, AgeRatingRequest ageRating);
        Task<bool> DeleteAgeRatingAsync(Guid id);
    }
}
