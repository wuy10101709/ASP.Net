using Microsoft.EntityFrameworkCore;
using Travel.Data;
using Travel.Models;
using Travel.Repositories;

namespace Travel.Modules.Rooms;

public class RoomRepository : BaseRepository<Room>, IRoomRepository
{
    public RoomRepository(AppDbContext context) : base(context) {}

    public async Task<IEnumerable<Room>> GetAllWithDetailsAsync(
        int? accommodationId,
        string? roomType,
        decimal? pricePerNight,
        string? keyword)
    {
        var query = _context.Rooms.Include(r => r.Accommodation).AsQueryable();

        if (accommodationId.HasValue)
            query = query.Where(r => r.AccommodationId == accommodationId.Value);

        if (!string.IsNullOrEmpty(roomType))
            query = query.Where(r => r.RoomType.ToLower().Contains(roomType.ToLower()));

        if (pricePerNight.HasValue)
            query = query.Where(r => r.PricePerNight == pricePerNight.Value);

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(r => r.RoomType.ToLower().Contains(keyword.ToLower())
                                  || r.Accommodation.Name.ToLower().Contains(keyword.ToLower()));

        return await query.ToListAsync();
    }

    // 1. Lấy danh sách phòng theo AccommodationId
    public async Task<IEnumerable<Room>> GetByAccommodationIdAsync(int accommodationId) =>
        await _context.Rooms
            .Where(r => r.AccommodationId == accommodationId)
            .OrderBy(r => r.PricePerNight)
            .ToListAsync();

    // 2. Lấy chi tiết phòng kèm Accommodation
    public async Task<Room?> GetByIdWithDetailsAsync(int id) =>
        await _context.Rooms
            .Include(r => r.Accommodation)
            .ThenInclude(a => a.Provider)
            .FirstOrDefaultAsync(r => r.Id == id);

    // 3. Kiểm tra phòng còn trống không
    public async Task<bool> HasAvailableRoomsAsync(int roomId, int numberOfRooms)
    {
        var room = await _context.Rooms.FindAsync(roomId);
        if (room == null) return false;
        return (room.TotalRooms - room.BookedRooms) >= numberOfRooms;
    }

    // Lấy phòng kèm Accommodation
public async Task<Room?> GetByIdWithAccommodationAsync(int roomId) =>
    await _context.Rooms
        .Include(r => r.Accommodation)
        .FirstOrDefaultAsync(r => r.Id == roomId);

// Kiểm tra phòng có booking active không
public async Task<bool> HasActiveBookingsAsync(int roomId) =>
    await _context.RoomBookings
        .AnyAsync(rb => rb.RoomId == roomId
                     && rb.Status != "Cancelled");
}