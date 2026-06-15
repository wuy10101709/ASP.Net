namespace Travel.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Tour | Accommodation
    public string Description { get; set; } = string.Empty;

    public ICollection<Tour> Tours { get; set; } = new List<Tour>();
    public ICollection<Accommodation> Accommodations { get; set; } = new List<Accommodation>();
}