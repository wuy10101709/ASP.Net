using Microsoft.EntityFrameworkCore;
using Travel.Data;
using Travel.Models;

namespace Travel.Modules.Reviews;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Review review)
        => await _context.Reviews.AddAsync(review);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();

    // Kiểm tra đã review booking tour chưa
    public async Task<bool> HasReviewedTourBookingAsync(int tourBookingId)
        => await _context.Reviews
            .AnyAsync(r => r.TourBookingId == tourBookingId);

    // Kiểm tra đã review booking phòng chưa
    public async Task<bool> HasReviewedRoomBookingAsync(int roomBookingId)
        => await _context.Reviews
            .AnyAsync(r => r.RoomBookingId == roomBookingId);

    // Lấy review của Tour kèm User
    public async Task<IEnumerable<Review>> GetByTourIdAsync(int tourId)
        => await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.TourBooking)
                .ThenInclude(tb => tb!.Tour)
            .Where(r => r.TargetType == "Tour" && r.TargetId == tourId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    // Lấy review của Accommodation kèm User
    public async Task<IEnumerable<Review>> GetByAccommodationIdAsync(int accommodationId)
        => await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.RoomBooking)
                .ThenInclude(rb => rb!.Room)
            .Where(r => r.TargetType == "Accommodation"
                     && r.TargetId == accommodationId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<Review?> GetByIdAsync(int id)
        => await _context.Reviews.FindAsync(id);

    public async Task DeleteAsync(Review review)
        => _context.Reviews.Remove(review);
}