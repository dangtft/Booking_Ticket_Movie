using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Orders;
using Microsoft.EntityFrameworkCore;
using Booking_Movie_Tickets.DTOs.Extras.Request;

namespace Booking_Movie_Tickets.Services
{
    public class ExtraService : IExtraService
    {
        private readonly BookingDbContext _context;

        public ExtraService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<Extra> GetExtraByIdAsync(Guid extraId)
        {
            return await _context.Extras.FirstOrDefaultAsync(e => e.Id == extraId && !e.IsDeleted);
        }

        public async Task<decimal> GetPriceAsync(Guid extraId)
        {
            var extra = await _context.Extras.FirstOrDefaultAsync(e => e.Id == extraId && !e.IsDeleted);
            return extra?.Price ?? 0;
        }

        public async Task<PagedResult<Extra>> GetAllExtrasAsync(PagedFilterBase filter)
        {
            var query = _context.Extras
                .Where(e => !e.IsDeleted)
                .Join(
                    _context.ExtrasCategories,  
                    e => e.CategoryId,
                    c => c.Id,
                    (e, c) => new Extra
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Description = e.Description,
                        Price = e.Price,
                        CategoryId = e.CategoryId
                    }
                );

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

            var extras = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<Extra>
            {
                Data = extras,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }


        public async Task<Extra> CreateExtraAsync(ExtraRequest request)
        {
            try
            {
                var extra = new Extra
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    CategoryId = request.CategoryId,
                    IsDeleted = false,
                };

                _context.Extras.Add(extra);
                await _context.SaveChangesAsync();
                return extra;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateExtraAsync(Guid extraId, ExtraRequest request)
        {
            try
            {
                var existingExtra = await _context.Extras.FindAsync(extraId);
                if (existingExtra == null || existingExtra.IsDeleted)
                    return false;

                existingExtra.Name = request.Name;
                existingExtra.Description = request.Description;
                existingExtra.Price = request.Price;
                existingExtra.CategoryId = request.CategoryId;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating extra: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteExtraAsync(Guid extraId)
        {
            var extra = await _context.Extras.FindAsync(extraId);
            if (extra == null || extra.IsDeleted) return false;

            extra.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}