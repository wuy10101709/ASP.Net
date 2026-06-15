using Microsoft.EntityFrameworkCore;
using Travel.Data;
using Travel.Models;
using Travel.DTOs;
using Travel.Repositories;

namespace Travel.Modules.Accommodations;

public class AccommodationRepository : BaseRepository<Accommodation>,IAccommodationRepository{

    public AccommodationRepository(AppDbContext context) : base(context){}
    public async Task<IEnumerable<Accommodation>> GetAccommodations(
            string? location,
            int? categoryId,
            float? minStar,
            string? keyword)
            {
        
        var query = _context.Accommodations
            .Include(a => a.Provider)
            .Include(a => a.Category)
            .Include(a => a.Rooms)
            .AsQueryable();

        if (!string.IsNullOrEmpty(location))
            query = query.Where(a => a.Location == location);

        if (categoryId.HasValue)
            query = query.Where(a => a.CategoryId == categoryId);

        if (minStar.HasValue)
            query = query.Where(a => a.StarRating >= minStar);

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(a => a.Name.Contains(keyword));

            return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
        }

        public async Task<Accommodation?> GetByIdWithDetailsAsync(int id) =>
            await _context.Accommodations
            .Include(a => a.Provider)
            .Include(a => a.Category)
            .Include(a => a.Rooms)
            .FirstOrDefaultAsync(a => a.Id == id);
        
        public async Task<IEnumerable<Accommodation>> GetByProviderIdAsync(int providerId) =>
            await _context.Accommodations
            .Include(t => t.Category)
            .Include(t => t.Provider)
            .Include(t => t.Rooms)
            .Where(t => t.ProviderId == providerId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
        

        public async Task<bool> HasActiveBookingsAsync(int accomodationId) =>
            await _context.RoomBookings
            .AnyAsync(rb => rb.Room.AccommodationId == accomodationId && rb.Status == "Active");
        

}