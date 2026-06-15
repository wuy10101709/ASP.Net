using Travel.Models;
using Travel.Data;
using Microsoft.EntityFrameworkCore;

using Travel.Repositories;

namespace Travel.Modules.TourBookings;

public class TourBookingRepository : BaseRepository<TourBooking>, ITourBookingRepository {
    public TourBookingRepository(AppDbContext context) : base(context) { }
    public async Task<bool> HasUserBookedTourAsync(int userId, int tourId)=>
        await _context.TourBookings.AnyAsync(b=> b.UserId == userId 
                                                    && b.TourId == tourId
                                                    && b.Status != "Cancelled");
    

    public async Task<TourBooking?> GetBookingIdWithDetailsAsync(int bookingId)=>
         await _context.TourBookings.Include(b => b.Tour)
                                          .Include(b=> b.User)
                                        .FirstOrDefaultAsync(b => b.Id == bookingId);

    public async Task<IEnumerable<TourBooking>> GetBookingsByProviderAsync(int providerId, string? status){
        var query= _context.TourBookings.Include(b=>b.Tour)
                                  .Include(b=> b.User)
                                  .Where(b=> b.Tour.ProviderId == providerId);

        if(!string.IsNullOrEmpty(status)){
            query = query.Where(b => b.Status == status);
        }

        return await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
    }

    public async Task<IEnumerable<TourBooking>> GetCustomerHistoryWithFilterAsync(int userId, int? tourId){
        var query = _context.TourBookings.Include(b=>b.Tour).Where(b=> b.UserId == userId);

        if(tourId.HasValue){
                query = query.Where(b=> b.TourId == tourId.Value);
        }

        return await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
    }

    public async Task<TourBooking?> GetBookingByTourAsync(int id) => 
        await _context.TourBookings.Include(b=> b.Tour).FirstOrDefaultAsync(b=> b.Id == id);
}