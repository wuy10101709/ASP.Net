namespace Travel.DTOs;

// ── Tour Booking ──────────────────────────────────────────
public class CreateTourBookingDto
{
    public int TourId { get; set; }
    public int NumberOfPeople { get; set; }
    public string Note { get; set; } = string.Empty;
}

public class TourBookingResponseDto
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public int NumberOfPeople { get; set; }
    public decimal TotalPrice { get; set; }
    public string Note { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Tour info
    public int TourId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string TourLocation { get; set; } = string.Empty;
    public DateTime DepartureDate { get; set; }

    // Tourist info
    public string TouristName { get; set; } = string.Empty;
    public string TouristEmail { get; set; } = string.Empty;
    public string TouristPhone { get; set; } = string.Empty;
}

// ── Room Booking ──────────────────────────────────────────
public class CreateRoomBookingDto
{
    public int RoomId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int NumberOfRooms { get; set; }
    public string Note { get; set; } = string.Empty;
}

public class RoomBookingResponseDto
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int NumberOfNights { get; set; }
    public int NumberOfRooms { get; set; }
    public decimal TotalPrice { get; set; }
    public string Note { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Room info
    public int RoomId { get; set; }
    public string RoomType { get; set; } = string.Empty;
    public string AccommodationName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    // Tourist info
    public string TouristName { get; set; } = string.Empty;
    public string TouristEmail { get; set; } = string.Empty;
    public string TouristPhone { get; set; } = string.Empty;
}