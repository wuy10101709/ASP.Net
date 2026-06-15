using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Travel.DTOs;
using Travel.Models;
using Travel.Repositories;
using Travel.Modules.Users ;
using Travel.Modules.Providers;

namespace Travel.Modules.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IProviderRepository _providerRepo;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepo,
        IProviderRepository providerRepo,
        IConfiguration configuration)
    {
        _userRepo = userRepo;
        _providerRepo = providerRepo;
        _configuration = configuration;
    }

    public async Task<(bool Success, string Message, UserResponseDto? Data)> RegisterAsync(
        RegisterDto dto)
    {   
       
        // Kiểm tra email trùng
        if (await _userRepo.EmailExistsAsync(dto.Email))
            return (false, "Email đã tồn tại.", null);

        // Chỉ cho phép Tourist hoặc Provider
        var role = dto.Role == "Provider" ? "Provider" : "Tourist";

        // Nếu Provider phải có tên công ty
        if (role == "Provider" && string.IsNullOrWhiteSpace(dto.CompanyName))
            return (false, "Vui lòng nhập tên công ty.", null);

        // Tạo User
        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Phone = dto.Phone,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();

        // Nếu Provider → tạo thêm hồ sơ Provider
        if (role == "Provider")
        {
            var provider = new Provider
            {
                UserId = user.Id,
                CompanyName = dto.CompanyName!,
                Address = dto.Address ?? string.Empty,
                Description = dto.CompanyDescription ?? string.Empty,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow
            };

            await _providerRepo.AddAsync(provider);
            await _providerRepo.SaveChangesAsync();
        }

        var token = CreateToken(user);

        return (true, "Đăng ký thành công.", new UserResponseDto
        {
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            Token = token
        });
    }

    public async Task<(bool Success, string Message, UserResponseDto? Data)> LoginAsync(
        LoginDto dto)
    {
        // Tìm user theo email
        var user = await _userRepo.GetByEmailAsync(dto.Email);

        if (user == null)
            return (false, "Email không tồn tại.", null);

        // Kiểm tra password
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return (false, "Sai mật khẩu.", null);

        var token = CreateToken(user);

        return (true, "Đăng nhập thành công.", new UserResponseDto
        {
           
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            Token = token
        });
    }

    // ── Tạo JWT Token ─────────────────────────────────────
    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}