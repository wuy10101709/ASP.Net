using Microsoft.EntityFrameworkCore;
using Travel.Data;
using Travel.Models;
using Travel.Repositories;

namespace Travel.Modules.RoomBookings;

public class RoomBookingRepository : BaseRepository<RoomBooking> , IRoomBookingRepository{
   public RoomBookingRepository(AppDbContext context) : base(context) {}
    //Xem chi tiết đơn đặt phòng kèm theo các bảng liên quan như Room, user
   public async Task<RoomBooking?> GetBookingByIdWithDetailsAsync(int id)=>
        await _context.RoomBookings.Include(rb => rb.User)
                                    .Include(rb => rb.Room)
                                    .ThenInclude(r => r.Accommodation)
                                    .FirstOrDefaultAsync(rb => rb.Id == id);

    //Tìm đơn đặt phòng theo id người dùng
    public async Task<IEnumerable<RoomBooking>> GetByUserIdAsync(int userId) =>
        await _context.RoomBookings.Include(rb => rb.Room)
                                    .Where(rb => rb.User.Id == userId)
                                    .OrderByDescending(b => b.CreatedAt)
                                    .ToListAsync();

    //Tìm các đơn đặt phòng thuộc về home,.. của nhà cung cấp đó
    public async Task<IEnumerable<RoomBooking>> GetByProviderIdAsync(int providerId, string? status){
        var query = _context.RoomBookings.Include(rb => rb.Room)
                                         .ThenInclude(r=> r.Accommodation)
                                         .Include(rb => rb.User)
                                         .Where(rb => rb.Room.Accommodation.ProviderId == providerId)
                                         .AsQueryable();

        if(!string.IsNullOrEmpty(status)){
            query = query.Where(rb => rb.Status == status);
        }

        return await query.OrderByDescending(rb => rb.CreatedAt).ToListAsync();
    }
}