using Microsoft.EntityFrameworkCore;
using Travel.Data;
using Travel.Models;
using Travel.Repositories;

namespace Travel.Modules.Admin;

public class AdminRepository : IAdminRepository
{
    private readonly AppDbContext _context;

    public AdminRepository(AppDbContext context)
    {
        _context = context;
    }

    // ── Provider ──────────────────────────────────────────
    public async Task<IEnumerable<Provider>> GetAllProvidersAsync(bool? isApproved)
    {
        var query = _context.Providers
            .Include(p => p.User)
            .AsQueryable();

        if (isApproved.HasValue)
            query = query.Where(p => p.IsApproved == isApproved);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Provider?> GetProviderByIdAsync(int id) =>
        await _context.Providers.FindAsync(id);

    // ── User ──────────────────────────────────────────────
    public async Task<IEnumerable<User>> GetAllUsersAsync(string? role)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(role))
            query = query.Where(u => u.Role == role);

        return await query
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id) =>
        await _context.Users.FindAsync(id);

    public async Task DeleteUserAsync(User user)
    {
        _context.Users.Remove(user);
    }

    // ── Tour ──────────────────────────────────────────────
    public async Task<IEnumerable<Tour>> GetAllToursAsync() =>
        await _context.Tours
            .Include(t => t.Provider)
            .Include(t => t.Category)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

    public async Task<Tour?> GetTourByIdAsync(int id) =>
        await _context.Tours.FindAsync(id);

    public async Task DeleteTourAsync(Tour tour)
    {
        _context.Tours.Remove(tour);
    }

    // ── Booking ───────────────────────────────────────────
    public async Task<IEnumerable<TourBooking>> GetAllTourBookingsAsync(string? status)
    {
        var query = _context.TourBookings
            .Include(tb => tb.Tour)
            .Include(tb => tb.User)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(tb => tb.Status == status);

        return await query
            .OrderByDescending(tb => tb.CreatedAt)
            .ToListAsync();
    }

    // ── Stats ─────────────────────────────────────────────
    public async Task<int> CountUsersAsync() =>
        await _context.Users.CountAsync();

    public async Task<int> CountProvidersAsync() =>
        await _context.Providers.CountAsync();

    public async Task<int> CountApprovedProvidersAsync() =>
        await _context.Providers.CountAsync(p => p.IsApproved);

    public async Task<int> CountPendingProvidersAsync() =>
        await _context.Providers.CountAsync(p => !p.IsApproved);

    public async Task<int> CountToursAsync() =>
        await _context.Tours.CountAsync();

    public async Task<int> CountTourBookingsAsync() =>
        await _context.TourBookings.CountAsync();

    public async Task<int> CountRoomBookingsAsync() =>
        await _context.RoomBookings.CountAsync();

    public async Task<decimal> GetTotalRevenueAsync() =>
        await _context.Payments
            .Where(p => p.Status == "Success")
            .SumAsync(p => p.Amount);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}