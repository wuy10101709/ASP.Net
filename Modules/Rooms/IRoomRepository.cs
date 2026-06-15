using Travel.Models;
using Travel.Repositories;
namespace Travel.Modules.Rooms;

public interface IRoomRepository : IRepository<Room>
{
    // Xem tất cả phòng theo lọc khách sạn , Loại phòng , giá phòng 
    Task<IEnumerable<Room>> GetAllWithDetailsAsync(
        int? accommodationId,
        string? roomType,
        decimal? pricePerNight,
        string? keyword);
    // 1. Lấy danh sách phòng theo AccommodationId
    // Dùng khi: xem tất cả phòng của 1 khách sạn
    Task<IEnumerable<Room>> GetByAccommodationIdAsync(int accommodationId);

    // 2. Lấy chi tiết phòng kèm Accommodation
    // Dùng khi: RoomBookingService cần biết phòng thuộc khách sạn nào
    Task<Room?> GetByIdWithDetailsAsync(int id);

    // 3. Kiểm tra phòng còn trống không
    // Dùng khi: Tourist đặt phòng — kiểm tra BookedRooms < TotalRooms
    Task<bool> HasAvailableRoomsAsync(int roomId, int numberOfRooms);

    //Lấy phòng kèm Accommodation để kiểm tra Provider
    Task<Room?> GetByIdWithAccommodationAsync(int roomId);

    // Kiểm tra phòng có booking active không
    Task<bool> HasActiveBookingsAsync(int roomId);
}