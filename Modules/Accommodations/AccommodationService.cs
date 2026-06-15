using Travel.DTOs;
using Travel.Models;
using Travel.Repositories;
using Travel.Modules.Rooms;
using Travel.Modules.Categorys;
using Travel.Modules.Providers;

namespace Travel.Modules.Accommodations;

public class AccommodationService : IAccommodationService
{
    private readonly IAccommodationRepository _AccommodationsRepo;
    private readonly IProviderRepository _providerRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IRoomRepository _roomRepo; // ← thêm

    public AccommodationService(
        IAccommodationRepository AccommodationsRepo,
        IProviderRepository providerRepo,
        ICategoryRepository categoryRepo,
        IRoomRepository roomRepo) // ← thêm
    {
        _AccommodationsRepo = AccommodationsRepo;
        _providerRepo = providerRepo;
        _categoryRepo = categoryRepo;
        _roomRepo = roomRepo; // ← thêm
    }

    // 1. Lấy danh sách
    public async Task<IEnumerable<AccommodationResponseDto>> GetAllAccommodationsAsync(
        string? location, int? categoryId, float? minStar, string? keyword)
    {
        var Accommodationss = await _AccommodationsRepo
            .GetAccommodations(location, categoryId, minStar, keyword);
        return Accommodationss.Select(MapToDto);
    }

    // 2. Chi tiết
    public async Task<AccommodationResponseDto?> GetAccommodationByIdAsync(int id)
    {
        var Accommodations = await _AccommodationsRepo.GetByIdWithDetailsAsync(id);
        return Accommodations == null ? null : MapToDto(Accommodations);
    }

    // 3. Tạo mới
    public async Task<AccommodationResponseDto> CreateAccommodationAsync(
        int userId, CreateAccommodationDto dto)
    {
        var provider = await _providerRepo.GetByUserIdAsync(userId)
            ?? throw new KeyNotFoundException("Không tìm thấy Provider.");

        if (!provider.IsApproved)
            throw new InvalidOperationException("Provider chưa được Admin duyệt.");

        var category = await _categoryRepo.IsValidCategoryAsync(dto.CategoryId,"Accommodation");
        if (!category)
            {
                throw new ArgumentException($"Danh mục có Id = {dto.CategoryId} không tồn tại hoặc không thuộc loại 'Accommodation'.");
            }

        var Accommodations = new Accommodation
        {
            ProviderId   = provider.Id, // ← fix: thiếu ProviderId
            CategoryId   = dto.CategoryId,
            Name         = dto.Name,
            Location     = dto.Location,
            Address      = dto.Address,
            Description  = dto.Description,
            ImageUrl     = dto.ImageUrl,
            StarRating   = dto.StarRating,
            CreatedAt    = DateTime.UtcNow,
            Rooms = dto.Rooms.Select(r => new Room
            {
                RoomType     = r.RoomType,
                PricePerNight= r.PricePerNight,
                Capacity     = r.Capacity,
                TotalRooms   = r.TotalRooms,
                Description  = r.Description
            }).ToList()
        };

        await _AccommodationsRepo.AddAsync(Accommodations);
        await _AccommodationsRepo.SaveChangesAsync();

        var result = await _AccommodationsRepo.GetByIdWithDetailsAsync(Accommodations.Id);
        return MapToDto(result!);
    }

    // 4. Cập nhật
    public async Task<bool> UpdateAccommodationAsync(
        int userId, int AccommodationsId, CreateAccommodationDto dto)
    {
        var provider = await _providerRepo.GetByUserIdAsync(userId);
        if (provider == null) return false;

        var Accommodations = await _AccommodationsRepo.GetByIdAsync(AccommodationsId);
        if (Accommodations == null || Accommodations.ProviderId != provider.Id)
            return false;

        // ✅ Sửa trực tiếp object đã load — không tạo object mới
        Accommodations.CategoryId  = dto.CategoryId;
        Accommodations.Name        = dto.Name;
        Accommodations.Location    = dto.Location;
        Accommodations.Address     = dto.Address;
        Accommodations.Description = dto.Description;
        Accommodations.ImageUrl    = dto.ImageUrl;
        Accommodations.StarRating  = dto.StarRating;

        await _AccommodationsRepo.UpdateAsync(Accommodations);
        await _AccommodationsRepo.SaveChangesAsync();
        return true;
    }

    // 5. Xóa
    public async Task<bool> DeleteAccommodationAsync(int userId, int AccommodationsId)
    {
        var provider = await _providerRepo.GetByUserIdAsync(userId);
        if (provider == null) return false;

        var Accommodations = await _AccommodationsRepo.GetByIdAsync(AccommodationsId);
        if (Accommodations == null || Accommodations.ProviderId != provider.Id)
            return false;

        await _AccommodationsRepo.DeleteAsync(Accommodations);
        await _AccommodationsRepo.SaveChangesAsync();
        return true; // ← fix: bỏ tuple
    }

    // 6. Thêm phòng
    public async Task<RoomResponseDto> AddRoomAsync(
        int userId, int AccommodationsId, CreateRoomDto dto)
    {
        var provider = await _providerRepo.GetByUserIdAsync(userId)
            ?? throw new KeyNotFoundException("Không tìm thấy Provider.");

        var Accommodations = await _AccommodationsRepo.GetByIdAsync(AccommodationsId)
            ?? throw new KeyNotFoundException("Không tìm thấy lưu trú.");

        if (Accommodations.ProviderId != provider.Id)
            throw new UnauthorizedAccessException("Bạn không có quyền.");

        var room = new Room
        {
            AccommodationId = AccommodationsId, // ← fix: dùng AccommodationsId
            RoomType        = dto.RoomType,
            PricePerNight   = dto.PricePerNight,
            Capacity        = dto.Capacity,
            TotalRooms      = dto.TotalRooms,
            BookedRooms     = 0,
            Description     = dto.Description
        };

        await _roomRepo.AddAsync(room);       // ← fix: dùng _roomRepo
        await _roomRepo.SaveChangesAsync();   // ← fix: dùng _roomRepo

        return new RoomResponseDto           // ← fix: return đầy đủ
        {
            Id            = room.Id,
            RoomType      = room.RoomType,
            PricePerNight = room.PricePerNight,
            Capacity      = room.Capacity,
            TotalRooms    = room.TotalRooms,
            BookedRooms   = room.BookedRooms,
            Description   = room.Description
        };
    }

    // 7. Tour của tôi
    public async Task<IEnumerable<AccommodationResponseDto>> GetMyAccommodationsAsync(int userId)
    {
        var provider = await _providerRepo.GetByUserIdAsync(userId)
            ?? throw new KeyNotFoundException("Không tìm thấy Provider.");

        var Accommodationss = await _AccommodationsRepo.GetByProviderIdAsync(provider.Id);
        return Accommodationss.Select(MapToDto); // ← fix: thêm logic
    }

    // Helper Map
    private AccommodationResponseDto MapToDto(Accommodation a) => new()
    {
        Id           = a.Id,
        Name         = a.Name,
        Location     = a.Location,
        Address      = a.Address,
        Description  = a.Description,
        ImageUrl     = a.ImageUrl,
        StarRating   = a.StarRating,
        CreatedAt    = a.CreatedAt,
        CategoryName = a.Category?.Name ?? "N/A",
        ProviderName = a.Provider?.CompanyName ?? "N/A",
        Rooms = a.Rooms?.Select(r => new RoomResponseDto
        {
            Id            = r.Id,
            RoomType      = r.RoomType,
            PricePerNight = r.PricePerNight,
            Capacity      = r.Capacity,
            TotalRooms    = r.TotalRooms,
            BookedRooms   = r.BookedRooms,
            Description   = r.Description
        }).ToList() ?? new List<RoomResponseDto>()
    };

    // 8. Cập nhật phòng
public async Task<bool> UpdateRoomAsync(int userId, int roomId, CreateRoomDto dto)
{
    // Lấy phòng kèm Accommodations để biết thuộc Provider nào
    var room = await _roomRepo.GetByIdWithAccommodationAsync(roomId)
        ?? throw new KeyNotFoundException("Phòng không tồn tại.");

    // Kiểm tra Provider có quyền không
    var provider = await _providerRepo.GetByUserIdAsync(userId)
        ?? throw new KeyNotFoundException("Không tìm thấy Provider.");

    if (room.Accommodation.ProviderId != provider.Id)
        throw new UnauthorizedAccessException("Bạn không có quyền sửa phòng này.");

    // Cập nhật trực tiếp — không tạo object mới
    room.RoomType      = dto.RoomType;
    room.PricePerNight = dto.PricePerNight;
    room.Capacity      = dto.Capacity;
    room.TotalRooms    = dto.TotalRooms;
    room.Description   = dto.Description;

    await _roomRepo.UpdateAsync(room);
    await _roomRepo.SaveChangesAsync();
    return true;
}

// 9. Xóa phòng
public async Task<(bool Success, string Message)> DeleteRoomAsync(int userId, int roomId)
{
    var room = await _roomRepo.GetByIdWithAccommodationAsync(roomId)
        ?? throw new KeyNotFoundException("Phòng không tồn tại.");

    var provider = await _providerRepo.GetByUserIdAsync(userId)
        ?? throw new KeyNotFoundException("Không tìm thấy Provider.");

    if (room.Accommodation.ProviderId != provider.Id)
        throw new UnauthorizedAccessException("Bạn không có quyền xóa phòng này.");

    // Không cho xóa nếu còn booking active
    var hasBooking = await _roomRepo.HasActiveBookingsAsync(roomId);
    if (hasBooking)
        return (false, "Không thể xóa phòng đang có booking.");

    await _roomRepo.DeleteAsync(room);
    await _roomRepo.SaveChangesAsync();
    return (true, "Xóa phòng thành công.");
}
}