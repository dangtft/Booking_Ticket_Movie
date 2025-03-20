using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Orders;
using Microsoft.EntityFrameworkCore;

namespace Booking_Movie_Tickets.Services
{
    public class ExtraService : IExtraService
    {
        private readonly BookingDbContext _context;

        public ExtraService(BookingDbContext context)
        {
            _context = context;
        }

        public Extra GetExtraById(Guid extraId)
        {
            return _context.Extras.FirstOrDefault(e => e.Id == extraId && !e.IsDeleted);
        }

        public decimal GetPrice(Guid extraId)
        {
            var extra = _context.Extras.FirstOrDefault(e => e.Id == extraId && !e.IsDeleted);
            return extra?.Price ?? 0;
        }

        public async Task<PagedResult<Extra>> GetAllExtrasAsync(PagedFilterBase filter)
        {
            var query = _context.Extras
                .Include(e => e.Category)
                .Where(e => !e.IsDeleted).AsQueryable();

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
            //                query = query.Where(m => EF.Functions.Like(m.Name, $"%{value}%"));
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
            //                        "name" => isAscending ? query.OrderBy(m => m.Name) :
            //                                  isDescending ? query.OrderByDescending(m => m.Name) : query,
            //                        "category" => isAscending ? query.OrderBy(m => m.Category.CategoryName) :
            //                        isDescending ? query.OrderByDescending(m => m.Category.CategoryName) : query,
            //                        _ => query
            //                    };
            //                }
            //                break;
            //        }
            //    }
            //}

            var extras = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();


            return new PagedResult<Extra>
            {
                Data = extras,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

    }
}
