// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.IdentityModel.Tokens;
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;
// using Travel.Data;
// using Travel.Models;
// using BCrypt.Net;

// [Route("api/[controller]")]
// [ApiController]
// public class AuthController : ControllerBase
// {
//     private readonly AppDbContext _context;
//     private readonly IConfiguration _configuration;

//     public AuthController(AppDbContext context, IConfiguration configuration)
//     {
//         _context = context;
//         _configuration = configuration;
//     }

    

//     [HttpPost("login")]
//     public async Task<ActionResult<UserResponseDto>> Login(LoginDto request)
//     {
//         // 1. Tìm User theo Email
//         var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

//         if (user == null)
//         {
//             return BadRequest("Email không tồn tại.");
//         }

//         // 2. Kiểm tra mật khẩu (Sử dụng BCrypt để so sánh password thô với Hash trong DB)
//         // Nếu bạn chưa hash thì dùng so sánh bằng: if (user.PasswordHash != request.Password)
//         if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
//         {
//             return BadRequest("Sai mật khẩu.");
//         }

//         // 3. Tạo Token JWT
//         string token = CreateToken(user);

//         return Ok(new UserResponseDto
//         {
//             FullName = user.FullName,
//             Email = user.Email,
//             Role = user.Role,
//             Token = token
//         });
//     }

//     private string CreateToken(User user)
//     {
//         List<Claim> claims = new List<Claim>
//         {
//             new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//             new Claim(ClaimTypes.Name, user.FullName),
//             new Claim(ClaimTypes.Email, user.Email),
//             new Claim(ClaimTypes.Role, user.Role),
//         };

//         var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
//             _configuration.GetSection("Jwt:Key").Value!));

//         var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

//         var token = new JwtSecurityToken(
//                 issuer: _configuration["Jwt:Issuer"],
//                 audience: _configuration["Jwt:Audience"],
//                 claims: claims,
//                 expires: DateTime.Now.AddDays(1),
//                 signingCredentials: creds
//             );

//         return new JwtSecurityTokenHandler().WriteToken(token);
//     }

//     [HttpPost("register")]
//     public async Task<ActionResult<User>> Register(RegisterDto request)
//     {   

//         if (await _context.Users.AnyAsync(u => u.Email == request.Email)){
//         return BadRequest("Email đã tồn tại.");
//         }   
//         //Chiir cho phép Tourist hoặc Provider
//         var role = request.Role == "Provider" ? "Provider" : "Tourist";

//         // 3. Nếu là Provider phải có tên công ty
//         if (role == "Provider" && string.IsNullOrWhiteSpace(request.CompanyName))
//             return BadRequest("Vui lòng nhập tên công ty.");

        
//         // Mã hóa mật khẩu trước khi lưu vào DB
//         string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        
//         // Tạo User
//         var newUser = new User
//         {
//         FullName = request.FullName,
//         Email = request.Email,
//         PasswordHash = passwordHash, // Lưu chuỗi đã mã hóa vào đây
//         Phone = request.Phone,
//         Role = request.Role,
//         CreatedAt = DateTime.Now
//         };

//         _context.Users.Add(newUser);
//         await _context.SaveChangesAsync();

//          // 6. Nếu là Provider → tạo thêm bản ghi Providers
//         if (role == "Provider")
//         {
//             var provider = new Provider
//             {
//                 UserId = newUser.Id,
//                 CompanyName = request.CompanyName!,
//                 Address = request.Address ?? string.Empty,
//                 Description = request.CompanyDescription ?? string.Empty,
//                 IsApproved = false, // Chờ Admin duyệt
//                 CreatedAt = DateTime.UtcNow
//             };
//             _context.Providers.Add(provider);
//             await _context.SaveChangesAsync();
//         }
        

//         // 7. Tạo token và trả về
//         string token = CreateToken(newUser);

//         return Ok(new UserResponseDto
//         {

//             FullName = newUser.FullName,
//             Email = newUser.Email,
//             Role = newUser.Role,
//             Token = token
//         });
//     }
// }

using Microsoft.AspNetCore.Mvc;
using Travel.DTOs;
using Travel.Modules.Auth;

namespace Travel.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto request)
    {
        var (success, message, data) = await _authService.RegisterAsync(request);

        if (!success)
            return BadRequest(message);

        return Ok(data);
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        var (success, message, data) = await _authService.LoginAsync(request);

        if (!success)
            return BadRequest(message);

        return Ok(data);
    }
}