public class RegisterDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = "Tourist"; // Tourist | Provider

    // Chỉ dùng khi Role = "Provider"
    public string? CompanyName { get; set; }
    public string? Address { get; set; }
    public string? CompanyDescription { get; set; }
}