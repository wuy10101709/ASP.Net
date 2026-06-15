using Travel.DTOs;
using Travel.Repositories;

namespace Travel.Modules.Admin;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepo;

    public AdminService(IAdminRepository adminRepo)
    {
        _adminRepo = adminRepo;
    }

    // ── Provider ──────────────────────────────────────────
    public async Task<IEnumerable<ProviderResponseDto>> GetAllProvidersAsync(bool? isApproved)
    {
        var providers = await _adminRepo.GetAllProvidersAsync(isApproved);

        return providers.Select(p => new ProviderResponseDto
        {
            Id          = p.Id,
            CompanyName = p.CompanyName,
            Address     = p.Address,
            Description = p.Description,
            IsApproved  = p.IsApproved,
            CreatedAt   = p.CreatedAt,
            Owner = new OwnerDto
            {
                Id       = p.User.Id,
                FullName = p.User.FullName,
                Email    = p.User.Email,
                Phone    = p.User.Phone
            }
        });
    }

    public async Task<(bool Success, string Message)> ApproveProviderAsync(int id)
    {
        var provider = await _adminRepo.GetProviderByIdAsync(id);

        if (provider == null)
            return (false, "Provider không tồn tại.");

        if (provider.IsApproved)
            return (false, "Provider này đã được duyệt rồi.");

        provider.IsApproved = true;
        await _adminRepo.SaveChangesAsync();

        return (true, $"Đã duyệt Provider '{provider.CompanyName}' thành công.");
    }

    public async Task<(bool Success, string Message)> RejectProviderAsync(int id)
    {
        var provider = await _adminRepo.GetProviderByIdAsync(id);

        if (provider == null)
            return (false, "Provider không tồn tại.");

        if (!provider.IsApproved)
            return (false, "Provider này chưa được duyệt.");

        provider.IsApproved = false;
        await _adminRepo.SaveChangesAsync();

        return (true, $"Đã thu hồi duyệt Provider '{provider.CompanyName}'.");
    }

    // ── User ──────────────────────────────────────────────
    public async Task<IEnumerable<AdminUserResponseDto>> GetAllUsersAsync(string? role)
    {
        var users = await _adminRepo.GetAllUsersAsync(role);

        return users.Select(u => new AdminUserResponseDto
        {
            Id        = u.Id,
            FullName  = u.FullName,
            Email     = u.Email,
            Phone     = u.Phone,
            Role      = u.Role,
            CreatedAt = u.CreatedAt
        });
    }

    public async Task<(bool Success, string Message)> DeleteUserAsync(int id)
    {
        var user = await _adminRepo.GetUserByIdAsync(id);

        if (user == null)
            return (false, "User không tồn tại.");

        if (user.Role == "Admin")
            return (false, "Không thể xóa tài khoản Admin.");

        await _adminRepo.DeleteUserAsync(user);
        await _adminRepo.SaveChangesAsync();

        return (true, "Đã xóa user thành công.");
    }

    // ── Tour ──────────────────────────────────────────────
    public async Task<IEnumerable<AdminTourResponseDto>> GetAllToursAsync()
    {
        var tours = await _adminRepo.GetAllToursAsync();

        return tours.Select(t => new AdminTourResponseDto
        {
            Id           = t.Id,
            Name         = t.Name,
            Location     = t.Location,
            Price        = t.Price,
            DepartureDate= t.DepartureDate,
            MaxSlots     = t.MaxSlots,
            BookedSlots  = t.BookedSlots,
            CreatedAt    = t.CreatedAt,
            CategoryName = t.Category?.Name ?? "",
            ProviderName = t.Provider?.CompanyName ?? ""
        });
    }

    public async Task<(bool Success, string Message)> DeleteTourAsync(int id)
    {
        var tour = await _adminRepo.GetTourByIdAsync(id);

        if (tour == null)
            return (false, "Tour không tồn tại.");

        await _adminRepo.DeleteTourAsync(tour);
        await _adminRepo.SaveChangesAsync();

        return (true, "Đã xóa tour thành công.");
    }

    // ── Booking ───────────────────────────────────────────
    public async Task<IEnumerable<AdminTourBookingResponseDto>> GetAllTourBookingsAsync(
        string? status)
    {
        var bookings = await _adminRepo.GetAllTourBookingsAsync(status);

        return bookings.Select(tb => new AdminTourBookingResponseDto
        {
            Id           = tb.Id,
            Status       = tb.Status,
            NumberOfPeople = tb.NumberOfPeople,
            TotalPrice   = tb.TotalPrice,
            Note         = tb.Note,
            CreatedAt    = tb.CreatedAt,
            TourName     = tb.Tour?.Name ?? "",
            TouristName  = tb.User?.FullName ?? "",
            TouristEmail = tb.User?.Email ?? ""
        });
    }

    // ── Stats ─────────────────────────────────────────────
    public async Task<AdminStatsDto> GetStatsAsync()
    {
        return new AdminStatsDto
        {
            TotalUsers         = await _adminRepo.CountUsersAsync(),
            TotalProviders     = await _adminRepo.CountProvidersAsync(),
            ApprovedProviders  = await _adminRepo.CountApprovedProvidersAsync(),
            PendingProviders   = await _adminRepo.CountPendingProvidersAsync(),
            TotalTours         = await _adminRepo.CountToursAsync(),
            TotalTourBookings  = await _adminRepo.CountTourBookingsAsync(),
            TotalRoomBookings  = await _adminRepo.CountRoomBookingsAsync(),
            TotalRevenue       = await _adminRepo.GetTotalRevenueAsync()
        };
    }
}