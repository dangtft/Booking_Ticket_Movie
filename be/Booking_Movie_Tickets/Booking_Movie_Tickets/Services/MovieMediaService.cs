using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.DTOs.Movies.Response;
using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Movies;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using AutoMapper;

namespace Booking_Movie_Tickets.Services
{
    public class MovieMediaService : IMovieMediaService
    {
        private readonly BookingDbContext _context;

        public MovieMediaService(BookingDbContext context)
        {
            _context = context;
        }

        // Lấy toàn bộ danh sách MovieMedia
        public async Task<PagedResult<MovieMedia>> GetAllMovieMediaAsync(PagedFilterBase filter)
        {
            var query = _context.MovieMedias.Where(m => !m.IsDeleted).AsQueryable();

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
            //                query = query.Where(m => EF.Functions.Like(m.MediaType, $"%{value}%"));
            //                break;

            //            case "sort":
            //                var sortParts = value.Split(':');
            //                if (sortParts.Length == 2)
            //                {
            //                    var field = sortParts[0].ToLower();
            //                    var order = sortParts[1].ToLower();

            //                    query = field switch
            //                    {
            //                        "type" => order == "asc" ? query.OrderBy(m => m.MediaType) : query.OrderByDescending(m => m.MediaType),
            //                        _ => query
            //                    };
            //                }
            //                break;
            //        }
            //    }
            //}

            var mediaList = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

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
            //                query = query.Where(m => EF.Functions.Like(m.MediaType, $"%{value}%"));
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
            //                        "type" => isAscending ? query.OrderBy(m => m.MediaType) :
            //                                  isDescending ? query.OrderByDescending(m => m.MediaType) : query,
            //                        _ => query
            //                    };
            //                }
            //                break;
            //        }
            //    }
            //}

            return new PagedResult<MovieMedia>
            {
                Data = mediaList,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }


        // Lấy MovieMedia theo ID
        public async Task<MovieMedia?> GetMovieMediaByIdAsync(Guid id)
        {
            return await _context.MovieMedias
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
        }

        // Lấy danh sách MovieMedia theo MovieId
        public async Task<List<MediaResponse?>> GetMovieMediaByMovieIdAsync(Guid movieId)
        {
            var mediaList = await _context.MovieMedias
                .Where(m => m.MovieId == movieId)
                .ToListAsync();

            var mediaResponseList = mediaList.Select(m => new MediaResponse
            {
                Id = m.Id,
                MovieId = m.MovieId,
                Description = m.Description,
                MediaType = m.MediaType,
                MediaURL = m.MediaURL
            }).ToList();

            return mediaResponseList;
        }

        // Thêm MovieMedia mới
        public async Task<MovieMedia> CreateMovieMediaAsync(MovieMedia media)
        {
            media.Id = Guid.NewGuid(); 
            media.IsDeleted = false;

            _context.MovieMedias.Add(media);
            await _context.SaveChangesAsync();
            return media;
        }

        // Cập nhật MovieMedia
        public async Task<MovieMedia?> UpdateMovieMediaAsync(Guid id, MovieMedia media)
        {
            var existingMedia = await _context.MovieMedias.FindAsync(id);
            if (existingMedia == null || existingMedia.IsDeleted)
                return null;

            existingMedia.Description = media.Description;
            existingMedia.MediaType = media.MediaType;
            existingMedia.MediaURL = media.MediaURL;

            await _context.SaveChangesAsync();
            return existingMedia;
        }

        // Xóa MovieMedia 
        public async Task<bool> DeleteMovieMediaAsync(Guid id)
        {
            var media = await _context.MovieMedias.FindAsync(id);
            if (media == null || media.IsDeleted)
                return false;

            media.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
