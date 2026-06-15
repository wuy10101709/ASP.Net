namespace Travel.Models;

public class Provider
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsApproved { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<Tour> Tours { get; set; } = new List<Tour>();
    public ICollection<Accommodation> Accommodations { get; set; } = new List<Accommodation>();
}