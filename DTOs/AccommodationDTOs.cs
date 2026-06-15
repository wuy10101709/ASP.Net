namespace Travel.DTOs;

public class CreateAccommodationDto
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public float StarRating { get; set; }
    public List<CreateRoomDto> Rooms { get; set; } = new();
}

public class CreateRoomDto
{
    
    public string RoomType { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public int TotalRooms { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class AccommodationResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public float StarRating { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public List<RoomResponseDto> Rooms { get; set; } = new();
}

public class RoomResponseDto
{
   public int Id { get; set; }
    public int AccommodationId { get; set; }
    public string AccommodationName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string AccommodationAddress { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public int TotalRooms { get; set; }
    public int BookedRooms { get; set; }
    public string Description { get; set; } = string.Empty;
}