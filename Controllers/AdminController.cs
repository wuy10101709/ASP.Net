// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Travel.Data;
// using Travel.DTOs;
// using Travel.Models;

// [Route("api/[controller]")]
// [ApiController]
// [Authorize(Roles = "Admin")]
// public class AdminController : ControllerBase
// {
//     private readonly AppDbContext _context;

//     public AdminController(AppDbContext context)
//     {
//         _context = context;
//     }

//     // ── PROVIDER MANAGEMENT ───────────────────────────────

//     // GET /api/admin/providers — Xem tất cả provider
//     [HttpGet("providers")]
//     public async Task<IActionResult> GetProviders([FromQuery] bool? isApproved)
//     {
//         var query = _context.Providers
//             .Include(p => p.User)
//             .AsQueryable();

//         if (isApproved.HasValue)
//             query = query.Where(p => p.IsApproved == isApproved);

//         var providers = await query
//             .OrderByDescending(p => p.CreatedAt)
//             .Select(p => new
//             {
//                 p.Id,
//                 p.CompanyName,
//                 p.Address,
//                 p.Description,
//                 p.IsApproved,
//                 p.CreatedAt,
//                 Owner = new
//                 {
//                     p.User.Id,
//                     p.User.FullName,
//                     p.User.Email,
//                     p.User.Phone
//                 }
//             })
//             .ToListAsync();

//         return Ok(providers);
//     }

//     // PATCH /api/admin/providers/{id}/approve — Duyệt provider
//     [HttpPatch("providers/{id}/approve")]
//     public async Task<IActionResult> ApproveProvider(int id)
//     {
//         var provider = await _context.Providers.FindAsync(id);

//         if (provider == null)
//             return NotFound("Provider không tồn tại.");

//         if (provider.IsApproved)
//             return BadRequest("Provider này đã được duyệt rồi.");

//         provider.IsApproved = true;
//         await _context.SaveChangesAsync();

//         return Ok(new { message = $"Đã duyệt Provider '{provider.CompanyName}' thành công." });
//     }

//     // PATCH /api/admin/providers/{id}/reject — Từ chối / thu hồi duyệt
//     [HttpPatch("providers/{id}/reject")]
//     public async Task<IActionResult> RejectProvider(int id)
//     {
//         var provider = await _context.Providers.FindAsync(id);

//         if (provider == null)
//             return NotFound("Provider không tồn tại.");

//         provider.IsApproved = false;
//         await _context.SaveChangesAsync();

//         return Ok(new { message = $"Đã thu hồi duyệt Provider '{provider.CompanyName}'." });
//     }

//     // ── USER MANAGEMENT ───────────────────────────────────

//     // GET /api/admin/users — Xem tất cả user
//     [HttpGet("users")]
//     public async Task<IActionResult> GetUsers([FromQuery] string? role)
//     {
//         var query = _context.Users.AsQueryable();

//         if (!string.IsNullOrEmpty(role))
//             query = query.Where(u => u.Role == role);

//         var users = await query
//             .OrderByDescending(u => u.CreatedAt)
//             .Select(u => new
//             {
//                 u.Id,
//                 u.FullName,
//                 u.Email,
//                 u.Phone,
//                 u.Role,
//                 u.CreatedAt
//             })
//             .ToListAsync();

//         return Ok(users);
//     }

//     // DELETE /api/admin/users/{id} — Xóa user
//     [HttpDelete("users/{id}")]
//     public async Task<IActionResult> DeleteUser(int id)
//     {
//         var user = await _context.Users.FindAsync(id);

//         if (user == null)
//             return NotFound("User không tồn tại.");

//         if (user.Role == "Admin")
//             return BadRequest("Không thể xóa tài khoản Admin.");

//         _context.Users.Remove(user);
//         await _context.SaveChangesAsync();

//         return Ok(new { message = "Đã xóa user thành công." });
//     }

//     // ── TOUR MANAGEMENT ───────────────────────────────────

//     // GET /api/admin/tours — Xem tất cả tour
//     [HttpGet("tours")]
//     public async Task<IActionResult> GetAllTours()
//     {
//         var tours = await _context.Tours
//             .Include(t => t.Provider)
//             .Include(t => t.Category)
//             .OrderByDescending(t => t.CreatedAt)
//             .Select(t => new
//             {
//                 t.Id,
//                 t.Name,
//                 t.Location,
//                 t.Price,
//                 t.DepartureDate,
//                 t.MaxSlots,
//                 t.BookedSlots,
//                 t.CreatedAt,
//                 Category = t.Category.Name,
//                 Provider = t.Provider.CompanyName
//             })
//             .ToListAsync();

//         return Ok(tours);
//     }

//     // DELETE /api/admin/tours/{id} — Admin xóa tour bất kỳ
//     [HttpDelete("tours/{id}")]
//     public async Task<IActionResult> DeleteTour(int id)
//     {
//         var tour = await _context.Tours.FindAsync(id);

//         if (tour == null)
//             return NotFound("Tour không tồn tại.");

//         _context.Tours.Remove(tour);
//         await _context.SaveChangesAsync();

//         return Ok(new { message = "Đã xóa tour thành công." });
//     }

//     // ── BOOKING MANAGEMENT ────────────────────────────────

//     // GET /api/admin/tour-bookings — Xem tất cả booking tour
//     [HttpGet("tour-bookings")]
//     public async Task<IActionResult> GetAllTourBookings([FromQuery] string? status)
//     {
//         var query = _context.TourBookings
//             .Include(tb => tb.Tour)
//             .Include(tb => tb.User)
//             .AsQueryable();

//         if (!string.IsNullOrEmpty(status))
//             query = query.Where(tb => tb.Status == status);

//         var bookings = await query
//             .OrderByDescending(tb => tb.CreatedAt)
//             .Select(tb => new
//             {
//                 tb.Id,
//                 tb.Status,
//                 tb.NumberOfPeople,
//                 tb.TotalPrice,
//                 tb.Note,
//                 tb.CreatedAt,
//                 Tour = tb.Tour.Name,
//                 Tourist = tb.User.FullName,
//                 TouristEmail = tb.User.Email
//             })
//             .ToListAsync();

//         return Ok(bookings);
//     }

//     // ── STATISTICS ────────────────────────────────────────

//     // GET /api/admin/stats — Thống kê tổng quan
//     [HttpGet("stats")]
//     public async Task<IActionResult> GetStats()
//     {
//         var totalUsers = await _context.Users.CountAsync();
//         var totalProviders = await _context.Providers.CountAsync();
//         var approvedProviders = await _context.Providers.CountAsync(p => p.IsApproved);
//         var pendingProviders = await _context.Providers.CountAsync(p => !p.IsApproved);
//         var totalTours = await _context.Tours.CountAsync();
//         var totalTourBookings = await _context.TourBookings.CountAsync();
//         var totalRoomBookings = await _context.RoomBookings.CountAsync();
//         var totalRevenue = await _context.Payments
//             .Where(p => p.Status == "Success")
//             .SumAsync(p => p.Amount);

//         return Ok(new
//         {
//             totalUsers,
//             totalProviders,
//             approvedProviders,
//             pendingProviders,
//             totalTours,
//             totalTourBookings,
//             totalRoomBookings,
//             totalRevenue
//         });
//     }
// }


    using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel.Modules.Admin;

namespace Travel.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    // ── Provider ──────────────────────────────────────────
    [HttpGet("providers")]
    public async Task<IActionResult> GetProviders([FromQuery] bool? isApproved)
    {
        var result = await _adminService.GetAllProvidersAsync(isApproved);
        return Ok(result);
    }

    [HttpPatch("providers/{id}/approve")]
    public async Task<IActionResult> ApproveProvider(int id)
    {
        var (success, message) = await _adminService.ApproveProviderAsync(id);
        if (!success) return BadRequest(message);
        return Ok(new { message });
    }

    [HttpPatch("providers/{id}/reject")]
    public async Task<IActionResult> RejectProvider(int id)
    {
        var (success, message) = await _adminService.RejectProviderAsync(id);
        if (!success) return BadRequest(message);
        return Ok(new { message });
    }

    // ── User ──────────────────────────────────────────────
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] string? role)
    {
        var result = await _adminService.GetAllUsersAsync(role);
        return Ok(result);
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var (success, message) = await _adminService.DeleteUserAsync(id);
        if (!success) return BadRequest(message);
        return Ok(new { message });
    }

    // ── Tour ──────────────────────────────────────────────
    [HttpGet("tours")]
    public async Task<IActionResult> GetAllTours()
    {
        var result = await _adminService.GetAllToursAsync();
        return Ok(result);
    }

    [HttpDelete("tours/{id}")]
    public async Task<IActionResult> DeleteTour(int id)
    {
        var (success, message) = await _adminService.DeleteTourAsync(id);
        if (!success) return NotFound(message);
        return Ok(new { message });
    }

    // ── Booking ───────────────────────────────────────────
    [HttpGet("tour-bookings")]
    public async Task<IActionResult> GetAllTourBookings([FromQuery] string? status)
    {
        var result = await _adminService.GetAllTourBookingsAsync(status);
        return Ok(result);
    }

    // ── Stats ─────────────────────────────────────────────
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var result = await _adminService.GetStatsAsync();
        return Ok(result);
    }
}