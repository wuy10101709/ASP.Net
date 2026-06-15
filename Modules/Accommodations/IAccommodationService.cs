using Travel.DTOs;


namespace Travel.Modules.Accommodations;

public interface IAccommodationService{
     // 1. Lấy danh sách cho khách xem (trả về DTO gọn nhẹ)
    Task<IEnumerable<AccommodationResponseDto>> GetAllAccommodationsAsync
    (string? location, int? categoryId, float? minStar, string? keyword);

    // 2. Xem chi tiết 1 khách sạn và các loại phòng bên trong
    Task<AccommodationResponseDto?> GetAccommodationByIdAsync(int id);

    // 3. Provider tạo mới khách sạn (Cần truyền UserId để tìm ProviderId)
    Task<AccommodationResponseDto> CreateAccommodationAsync(int userId, CreateAccommodationDto dto);

    // 4. Provider cập nhật thông tin
    Task<bool> UpdateAccommodationAsync(int userId, int accommodationId, CreateAccommodationDto dto);

    // 5. Xóa khách sạn
    Task<bool> DeleteAccommodationAsync(int userId, int accommodationId);

    // 6. Thêm phòng vào khách sạn (Sử dụng RoomRepository bên trong service này luôn)
    Task<RoomResponseDto> AddRoomAsync(int userId, int accommodationId, CreateRoomDto dto);

    // 7. Xem danh sách khách sạn riêng của tôi (Provider)
    Task<IEnumerable<AccommodationResponseDto>> GetMyAccommodationsAsync(int userId);

    //8.Cập nhập Phòng
    Task<bool> UpdateRoomAsync(int userId, int roomId, CreateRoomDto dto);

    //9. Xóa phòng trong lưu trú 
    Task<(bool Success, string Message)> DeleteRoomAsync(int userId, int roomId);
}
