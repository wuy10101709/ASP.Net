using Travel.DTOs;

namespace Travel.Modules.Tours;

public interface ITourService
{
    Task<IEnumerable<TourResponseDto>> GetAllToursAsync(
        string? location,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        string? keyword);

    Task<TourResponseDto?> GetTourByIdAsync(int id);
    Task<TourResponseDto> CreateTourAsync(int userId, CreateTourDto dto);
    Task<bool> UpdateTourAsync(int userId, int tourId, CreateTourDto dto);
    Task<(bool Success, string Message)> DeleteTourAsync(int userId, int tourId);
    Task<IEnumerable<TourResponseDto>> GetMyToursAsync(int userId);
}