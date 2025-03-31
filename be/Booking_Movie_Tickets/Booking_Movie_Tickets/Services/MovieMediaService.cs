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

            var totalCount = await query.CountAsync();

            var mediaList = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<MovieMedia>
            {
                Data = mediaList,
                Page = filter.Page,
                TotalCount = totalCount,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
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
