using Travel.DTOs;
using Travel.Models;
using Travel.Repositories;
using Travel.Modules.Categorys;
using Travel.Modules.Providers;
using Microsoft.EntityFrameworkCore;

namespace Travel.Modules.Tours;

public class TourService : ITourService
{
    private readonly ITourRepository _tourRepo;
    private readonly IProviderRepository _providerRepo;
    private readonly ICategoryRepository _categoryRepo; // ← thêm

    public TourService(
        ITourRepository tourRepo,
        IProviderRepository providerRepo,
        ICategoryRepository categoryRepo) // ← thêm
    {
        _tourRepo = tourRepo;
        _providerRepo = providerRepo;
        _categoryRepo = categoryRepo; // ← thêm
    }

    // 1. Lấy danh sách tour
    public async Task<IEnumerable<TourResponseDto>> GetAllToursAsync(
        string? location, int? categoryId,
        decimal? minPrice, decimal? maxPrice, string? keyword)
    {
        var tours = await _tourRepo.GetAllWithDetailsAsync(
            location, categoryId, minPrice, maxPrice, keyword);

        return tours.Select(MapToDto);
    }

    // 2. Xem chi tiết tour
    public async Task<TourResponseDto?> GetTourByIdAsync(int id)
    {
        var tour = await _tourRepo.GetByIdWithDetailsAsync(id);
        return tour == null ? null : MapToDto(tour);
    }

    // 3. Tạo tour
    public async Task<TourResponseDto> CreateTourAsync(int userId, CreateTourDto dto)
    {
        // Kiểm tra Provider
        var provider = await _providerRepo.GetByUserIdAsync(userId)
            ?? throw new InvalidOperationException("Không tìm thấy Provider.");

        if (!provider.IsApproved)
            throw new InvalidOperationException("Provider chưa được Admin duyệt.");

        // ← Thêm: Kiểm tra Category
        var category = await _categoryRepo.GetByIdAsync(dto.CategoryId);
        if (category == null || category.Type != "Tour")
            throw new ArgumentException("Danh mục tour không hợp lệ.");

        var tour = new Tour
        {
            ProviderId    = provider.Id,
            CategoryId    = dto.CategoryId,
            Name          = dto.Name,
            Location      = dto.Location,
            Price         = dto.Price,
            Description   = dto.Description,
            ImageUrl      = dto.ImageUrl,
            DepartureDate = dto.DepartureDate,
            DurationDays  = dto.DurationDays,
            MaxSlots      = dto.MaxSlots,
            BookedSlots   = 0,
            CreatedAt     = DateTime.UtcNow
        };

        await _tourRepo.AddAsync(tour);
        await _tourRepo.SaveChangesAsync();

        // Nạp dữ liệu chi tiết 1 lần duy nhất
        var created = await _tourRepo.GetByIdWithDetailsAsync(tour.Id);
        return MapToDto(created!);
    }

    // 4. Cập nhật tour
    public async Task<bool> UpdateTourAsync(int userId, int tourId, CreateTourDto dto)
    {
        var provider = await _providerRepo.GetByUserIdAsync(userId);
        if (provider == null) return false;

        var tour = await _tourRepo.GetByIdAsync(tourId);
        if (tour == null || tour.ProviderId != provider.Id) return false;

        tour.Name          = dto.Name;
        tour.CategoryId    = dto.CategoryId;
        tour.Location      = dto.Location;
        tour.Price         = dto.Price;
        tour.Description   = dto.Description;
        tour.ImageUrl      = dto.ImageUrl;
        tour.DepartureDate = dto.DepartureDate;
        tour.DurationDays  = dto.DurationDays;
        tour.MaxSlots      = dto.MaxSlots;

        await _tourRepo.UpdateAsync(tour);
        await _tourRepo.SaveChangesAsync();
        return true;
    }

    // 5. Xóa tour
    public async Task<(bool Success, string Message)> DeleteTourAsync(int userId, int tourId)
    {
        var provider = await _providerRepo.GetByUserIdAsync(userId);
        if (provider == null)
            return (false, "Không tìm thấy Provider.");

        var tour = await _tourRepo.GetByIdAsync(tourId);
        if (tour == null)
            return (false, "Tour không tồn tại.");

        // ← Fix: throw thay vì return string "Forbidden"
        if (tour.ProviderId != provider.Id)
            throw new UnauthorizedAccessException("Bạn không có quyền xóa tour này.");

        var hasBooking = await _tourRepo.HasActiveBookingsAsync(tourId);
        if (hasBooking)
            return (false, "Không thể xóa tour đã có booking.");

        await _tourRepo.DeleteAsync(tour);
        await _tourRepo.SaveChangesAsync();
        return (true, "Xóa tour thành công.");
    }

    // 6. Tour của Provider
    public async Task<IEnumerable<TourResponseDto>> GetMyToursAsync(int userId)
    {
        var provider = await _providerRepo.GetByUserIdAsync(userId)
            ?? throw new InvalidOperationException("Không tìm thấy Provider.");

        var tours = await _tourRepo.GetByProviderIdAsync(provider.Id);
        return tours.Select(MapToDto);
    }

    // ── Helper Map ────────────────────────────────────────
    private static TourResponseDto MapToDto(Tour t) => new()
    {
        Id            = t.Id,
        Name          = t.Name,
        Location      = t.Location,
        Price         = t.Price,
        Description   = t.Description,
        ImageUrl      = t.ImageUrl,
        DepartureDate = t.DepartureDate,
        DurationDays  = t.DurationDays,
        MaxSlots      = t.MaxSlots,
        BookedSlots   = t.BookedSlots,
        // AvailableSlots tự tính trong DTO — không cần set
        CreatedAt     = t.CreatedAt,
        CategoryName  = t.Category?.Name ?? "",
        ProviderName  = t.Provider?.CompanyName ?? ""
    };
}