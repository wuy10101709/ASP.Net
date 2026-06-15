

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Travel.DTOs;
using Travel.Modules.Accommodations;

namespace Travel.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccommodationsController : ControllerBase
{
    private readonly IAccommodationService _accommodationService;

    public AccommodationsController(IAccommodationService accommodationService)
    {
        _accommodationService = accommodationService;
    }

    // ── GET /api/accommodations ───────────────────────────
    // Ai cũng xem được — có filter
    [HttpGet]
    public async Task<IActionResult> GetAccommodations(
        [FromQuery] string? location,
        [FromQuery] int? categoryId,
        [FromQuery] float? minStar,
        [FromQuery] string? keyword)
    {
        var result = await _accommodationService
            .GetAllAccommodationsAsync(location, categoryId, minStar, keyword);

        return Ok(result);
    }

    // ── GET /api/accommodations/{id} ──────────────────────
    // Xem chi tiết 1 lưu trú kèm danh sách phòng
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccommodation(int id)
    {
        var result = await _accommodationService.GetAccommodationByIdAsync(id);

        if (result == null)
            return NotFound("Lưu trú không tồn tại.");

        return Ok(result);
    }

    // ── POST /api/accommodations ──────────────────────────
    // Provider tạo lưu trú mới
    [HttpPost]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> CreateAccommodation(CreateAccommodationDto request)
    {
        try
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _accommodationService
                .CreateAccommodationAsync(userId, request);

            return CreatedAtAction(nameof(GetAccommodation),
                new { id = result.Id }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // ── PUT /api/accommodations/{id} ──────────────────────
    // Provider cập nhật lưu trú của mình
    [HttpPut("{id}")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> UpdateAccommodation(
        int id, CreateAccommodationDto request)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _accommodationService
            .UpdateAccommodationAsync(userId, id, request);

        if (!result)
            return NotFound("Lưu trú không tồn tại hoặc bạn không có quyền.");

        return Ok("Cập nhật lưu trú thành công.");
    }

    // ── DELETE /api/accommodations/{id} ───────────────────
    // Provider xóa lưu trú của mình
    [HttpDelete("{id}")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> DeleteAccommodation(int id)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _accommodationService
            .DeleteAccommodationAsync(userId, id);

        if (!result)
            return NotFound("Lưu trú không tồn tại hoặc bạn không có quyền.");

        return Ok("Xóa lưu trú thành công.");
    }

    // ── POST /api/accommodations/{id}/rooms ───────────────
    // Provider thêm phòng vào lưu trú
   [HttpPost("{id}/rooms")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> AddRoom(int id, CreateRoomDto request)
    {
    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var result = await _accommodationService.AddRoomAsync(userId, id, request);
    return Ok(result);
    }

    // ── GET /api/accommodations/my-accommodations ─────────
    // Provider xem danh sách lưu trú của mình
    [HttpGet("my-accommodations")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> GetMyAccommodations()
    {
        try
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _accommodationService
                .GetMyAccommodationsAsync(userId);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // ── PUT /api/accommodations/rooms/{roomId} ────────────
// Provider cập nhật thông tin phòng
[HttpPut("rooms/{roomId}")]
[Authorize(Roles = "Provider")]
public async Task<IActionResult> UpdateRoom(int roomId, CreateRoomDto request)
{
    try
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _accommodationService
            .UpdateRoomAsync(userId, roomId, request);

        return Ok("Cập nhật phòng thành công.");
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(ex.Message);
    }
    catch (UnauthorizedAccessException)
    {
        return Forbid();
    }
}

// ── DELETE /api/accommodations/rooms/{roomId} ─────────
// Provider xóa phòng
[HttpDelete("rooms/{roomId}")]
[Authorize(Roles = "Provider")]
public async Task<IActionResult> DeleteRoom(int roomId)
{
    try
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var (success, message) = await _accommodationService
            .DeleteRoomAsync(userId, roomId);

        if (!success)
            return BadRequest(message);

        return Ok(message);
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(ex.Message);
    }
    catch (UnauthorizedAccessException)
    {
        return Forbid();
    }
}
}