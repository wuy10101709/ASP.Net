using Travel.DTOs;

namespace Travel.Modules.Rooms;

public interface IRoomService
{
    Task<IEnumerable<RoomResponseDto>> GetAllRoomsAsync(
        string? location,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        string? keyword);

    Task<RoomResponseDto?> GetRoomByIdAsync(int id);
    Task<RoomResponseDto> CreateRoomAsync(int userId, CreateRoomDto dto);
    Task<bool> UpdateRoomAsync(int userId, int roomId, CreateRoomDto dto);
    Task<(bool Success, string Message)> DeleteRoomAsync(int userId, int roomId);
    Task<IEnumerable<RoomResponseDto>> GetMyRoomsAsync(int userId);
}