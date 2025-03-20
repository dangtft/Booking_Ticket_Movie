using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Movies;
using Microsoft.EntityFrameworkCore;
using System;
using Booking_Movie_Tickets.DTOs.Movies.Request;
using Booking_Movie_Tickets.DTOs.Others;
namespace Booking_Movie_Tickets.Services
{
    public class AgeRatingService : IAgeRatingService
    {
        private readonly BookingDbContext _context;
        private readonly ILogger<AgeRatingService> _logger;

        public AgeRatingService(BookingDbContext context, ILogger<AgeRatingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PagedResult<AgeRating>> GetAllAgeRatingsAsync(AgeRatingFilter filter)
        {
            try
            {
                var query = _context.AgeRatings.AsQueryable();

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
                //                query = query.Where(m => EF.Functions.Like(m.RatingLabel, $"%{value}%") ||
                //                                         EF.Functions.Like(m.Description, $"%{value}%"));
                //                break;

                //            case "sort":
                //                var sortParts = value.Split(':');
                //                if (sortParts.Length == 2)
                //                {
                //                    var field = sortParts[0].ToLower();
                //                    var order = sortParts[1].ToLower();

                //                    query = field switch
                //                    {
                //                        "name" => order == "asc" ? query.OrderBy(m => m.RatingLabel) : query.OrderByDescending(m => m.RatingLabel),
                //                        _ => query
                //                    };
                //                }
                //                break;
                //        }
                //    }
                //}

                var totalItems = await query.CountAsync();

                var ageRatingData = await query
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync(); 

                return new PagedResult<AgeRating>
                {
                    Data = ageRatingData,
                    Page = filter.Page,
                    PageSize = filter.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách phim.");
                throw;
            }
        }


        public async Task<AgeRating> GetAgeRatingByIdAsync(Guid id)
        {
            return await _context.AgeRatings.FindAsync(id);
        }

        public async Task<AgeRating> AddAgeRatingAsync(AgeRatingRequest ageRating)
        {
            var newAgeRating = new AgeRating
            {
                Id = Guid.NewGuid(),
                RatingLabel = ageRating.RatingLabel,
                Description = ageRating.Description
            };
            _context.AgeRatings.Add(newAgeRating);
            await _context.SaveChangesAsync();
            return newAgeRating;
        }

        public async Task<AgeRating> UpdateAgeRatingAsync(Guid id, AgeRatingRequest ageRating)
        {
            var existing = await _context.AgeRatings.FindAsync(id);
            if (existing == null) return null;

            existing.RatingLabel = ageRating.RatingLabel;
            existing.Description = ageRating.Description;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAgeRatingAsync(Guid id)
        {
            var existing = await _context.AgeRatings.FindAsync(id);
            if (existing == null) return false;

            _context.AgeRatings.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
