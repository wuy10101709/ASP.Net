using Travel.DTOs;

namespace Travel.Modules.Admin;

public interface IAdminService
{
    // Provider
    Task<IEnumerable<ProviderResponseDto>> GetAllProvidersAsync(bool? isApproved);
    Task<(bool Success, string Message)> ApproveProviderAsync(int id);
    Task<(bool Success, string Message)> RejectProviderAsync(int id);

    // User
    Task<IEnumerable<AdminUserResponseDto>> GetAllUsersAsync(string? role);
    Task<(bool Success, string Message)> DeleteUserAsync(int id);

    // Tour
    Task<IEnumerable<AdminTourResponseDto>> GetAllToursAsync();
    Task<(bool Success, string Message)> DeleteTourAsync(int id);

    // Booking
    Task<IEnumerable<AdminTourBookingResponseDto>> GetAllTourBookingsAsync(string? status);

    // Stats
    Task<AdminStatsDto> GetStatsAsync();
}