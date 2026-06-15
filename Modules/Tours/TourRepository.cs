using Microsoft.EntityFrameworkCore;
using Travel.Data;
using Travel.Models;
using Travel.Repositories;
namespace Travel.Modules.Tours;

public class TourRepository : BaseRepository<Tour>, ITourRepository
{
    public TourRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Tour>> GetAllWithDetailsAsync(
        string? location,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        string? keyword)
    {
        var query = _context.Tours
            .Include(t => t.Provider)
            .Include(t => t.Category)
            .AsQueryable();

        if (!string.IsNullOrEmpty(location))
            query = query.Where(t => t.Location == location);

        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId);

        if (minPrice.HasValue)
            query = query.Where(t => t.Price >= minPrice);

        if (maxPrice.HasValue)
            query = query.Where(t => t.Price <= maxPrice);

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(t => t.Name.Contains(keyword));

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Tour?> GetByIdWithDetailsAsync(int id) =>
        await _context.Tours
            .Include(t => t.Provider)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IEnumerable<Tour>> GetByProviderIdAsync(int providerId) =>
        await _context.Tours
            .Include(t => t.Category)
            .Include(t => t.Provider)
            .Where(t => t.ProviderId == providerId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

    public async Task<bool> HasActiveBookingsAsync(int tourId) =>
        await _context.TourBookings
            .AnyAsync(tb => tb.TourId == tourId && tb.Status != "Cancelled");
}