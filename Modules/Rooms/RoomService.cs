using Travel.DTOs;
using Travel.Models;
using Travel.Modules.Providers;
using Travel.Shared.Exceptions;

namespace Travel.Modules.Rooms;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepo;
    private readonly IProviderRepository _providerRepo;

    public RoomService(
        IRoomRepository roomRepo,
        IProviderRepository providerRepo)
    {
        _roomRepo = roomRepo;
        _providerRepo = providerRepo;
    }

    // ── 1. Lấy danh sách phòng ───────────────────────────
    public async Task<IEnumerable<RoomResponseDto>> GetAllRoomsAsync(
        string? location,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        string? keyword)
    {
        // Repository lo filter — truyền đúng tham số
        var rooms = await _roomRepo.GetAllWithDetailsAsync(
            null,       // accommodationId — không lọc
            null,       // roomType
            maxPrice,   // maxPrice
            keyword);   // keyword

        // Filter thêm trên kết quả trả về
        var result = rooms.AsEnumerable();

        if (!string.IsNullOrEmpty(location))
            result = result.Where(r =>
                r.Accommodation?.Location
                 .ToLower()
                 .Contains(location.ToLower()) == true);

        if (categoryId.HasValue)
            result = result.Where(r =>
                r.Accommodation?.CategoryId == categoryId.Value);

        if (minPrice.HasValue)
            result = result.Where(r => r.PricePerNight >= minPrice.Value);

        if (maxPrice.HasValue)
            result = result.Where(r => r.PricePerNight <= maxPrice.Value);

        if (!string.IsNullOrEmpty(keyword))
        {
            var search = keyword.Trim().ToLower();
            result = result.Where(r =>
                r.RoomType.ToLower().Contains(search) ||
                (r.Accommodation?.Name.ToLower().Contains(search) == true));
        }

        return result.Select(MapToDto).ToList();
    }

    // ── 2. Xem chi tiết 1 phòng ──────────────────────────
    public async Task<RoomResponseDto?> GetRoomByIdAsync(int id)
    {
        var room = await _roomRepo.GetByIdWithDetailsAsync(id);
        return room == null ? null : MapToDto(room); // ← fix CS0161
    }

    // ── 3. Tạo phòng ─────────────────────────────────────
    public async Task<RoomResponseDto> CreateRoomAsync(
        int userId, CreateRoomDto dto)
    {
        var provider = await _providerRepo.GetByUserIdAsync(userId);
         if(provider == null)  throw new NotFoundException("ROOM_NOT_FOUND|Phòng yêu cầu không tồn tại trên hệ thống.");

        var room = new Room
        {
            
            RoomType        = dto.RoomType,
            PricePerNight   = dto.PricePerNight,
            Capacity        = dto.Capacity,
            TotalRooms      = dto.TotalRooms,
            BookedRooms     = 0,
            Description     = dto.Description
        };

        await _roomRepo.AddAsync(room);
        await _roomRepo.SaveChangesAsync();

        var created = await _roomRepo.GetByIdWithDetailsAsync(room.Id);
        return MapToDto(created!);
    }

    // ── 4. Cập nhật phòng ────────────────────────────────
    public async Task<bool> UpdateRoomAsync(
        int userId, int roomId, CreateRoomDto dto)
    {
        var provider = await _providerRepo.GetByUserIdAsync(userId);
        if (provider == null) return false;

        var room = await _roomRepo.GetByIdWithAccommodationAsync(roomId);
        if (room == null) return false;

        if (room.Accommodation?.ProviderId != provider.Id)
            throw new ForbiddenException("ErrorMessages.ProviderNoPermission|Bạn không có quyền sửa phòng này.");

        room.RoomType      = dto.RoomType;
        room.PricePerNight = dto.PricePerNight;
        room.Capacity      = dto.Capacity;
        room.TotalRooms    = dto.TotalRooms;
        room.Description   = dto.Description;

        await _roomRepo.UpdateAsync(room);
        await _roomRepo.SaveChangesAsync();
        return true;
    }

    // ── 5. Xóa phòng ─────────────────────────────────────
    public async Task<(bool Success, string Message)> DeleteRoomAsync(
        int userId, int roomId)
    {
        var provider = await _providerRepo.GetByUserIdAsync(userId);
        if (provider == null)
            return (false, "Không tìm thấy Provider.");

        var room = await _roomRepo.GetByIdWithAccommodationAsync(roomId);
        if(room == null) throw new NotFoundException("ErrorMessages.RoomNotFound|Phòng không tồn tại.");

        if (room.Accommodation?.ProviderId != provider.Id)
            throw new ForbiddenException("ErrorMessages.ProviderNoPermission|Bạn không có quyền xóa phòng này.");

        var hasBooking = await _roomRepo.HasActiveBookingsAsync(roomId);
        if (hasBooking)
            return (false, "Không thể xóa phòng đang có booking.");

        await _roomRepo.DeleteAsync(room);
        await _roomRepo.SaveChangesAsync();
        return (true, "Xóa phòng thành công.");
    }

    // ── 6. Phòng của Provider ─────────────────────────────
    public async Task<IEnumerable<RoomResponseDto>> GetMyRoomsAsync(int userId)
    {
        var provider = await _providerRepo.GetByUserIdAsync(userId);
        if(provider == null) throw new NotFoundException("ErrorMessages.ProviderNotFound|Không tìm thấy Provider.");

        var rooms = await _roomRepo
            .GetByAccommodationIdAsync(provider.Id);

        return rooms.Select(MapToDto);
    }

    // ── Helper Map ────────────────────────────────────────
    private static RoomResponseDto MapToDto(Room room) => new()
    {
        Id                  = room.Id,
        RoomType            = room.RoomType,
        PricePerNight       = room.PricePerNight,
        Capacity            = room.Capacity,
        TotalRooms          = room.TotalRooms,
        BookedRooms         = room.BookedRooms,
        Description         = room.Description,

        // Accommodation info — fix CS0117
        AccommodationId     = room.AccommodationId,
        AccommodationName   = room.Accommodation?.Name ?? "N/A",
        Location            = room.Accommodation?.Location ?? "N/A",
        AccommodationAddress= room.Accommodation?.Address ?? "N/A",
        StarRating = room.Accommodation != null ? (int)room.Accommodation.StarRating : 0,
        ProviderName        = room.Accommodation?.Provider?.CompanyName ?? "N/A"
    };
}