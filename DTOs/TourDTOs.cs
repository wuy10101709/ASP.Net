namespace Travel.DTOs;

// Dùng khi tạo / cập nhật tour (Provider gửi lên)
public class CreateTourDto
{
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string Location { get; set; } = string.Empty; // QuyNhon | PhuYen | Both
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime DepartureDate { get; set; }
    public int DurationDays { get; set; }
    public int MaxSlots { get; set; }
}

// Dùng khi trả về danh sách / chi tiết tour (gửi về client)
public class TourResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime DepartureDate { get; set; }
    public int DurationDays { get; set; }
    public int MaxSlots { get; set; }
    public int BookedSlots { get; set; }
    public int AvailableSlots => MaxSlots - BookedSlots; // Số chỗ còn lại
    public DateTime CreatedAt { get; set; }

    // Thông tin Category
    public string CategoryName { get; set; } = string.Empty;

    // Thông tin Provider
    public string ProviderName { get; set; } = string.Empty;
}