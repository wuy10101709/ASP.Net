namespace Travel.DTOs;

public class ProviderResponseDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
    public OwnerDto Owner { get; set; } = new();
}

public class OwnerDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class AdminUserResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class AdminTourResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime DepartureDate { get; set; }
    public int MaxSlots { get; set; }
    public int BookedSlots { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
}

public class AdminTourBookingResponseDto
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public int NumberOfPeople { get; set; }
    public decimal TotalPrice { get; set; }
    public string Note { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string TouristName { get; set; } = string.Empty;
    public string TouristEmail { get; set; } = string.Empty;
}

public class AdminStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalProviders { get; set; }
    public int ApprovedProviders { get; set; }
    public int PendingProviders { get; set; }
    public int TotalTours { get; set; }
    public int TotalTourBookings { get; set; }
    public int TotalRoomBookings { get; set; }
    public decimal TotalRevenue { get; set; }
}