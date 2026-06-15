

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Travel.DTOs;
using Travel.Modules.Tours;

[Route("api/[controller]")]
[ApiController]
public class ToursController : ControllerBase
{
    private readonly ITourService _tourService;

    public ToursController(ITourService tourService)
    {
        _tourService = tourService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTours(
        [FromQuery] string? location,
        [FromQuery] int? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? keyword)
    {
        var tours = await _tourService.GetAllToursAsync(
            location, categoryId, minPrice, maxPrice, keyword);
        return Ok(tours);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTour(int id)
    {
        var tour = await _tourService.GetTourByIdAsync(id);
        if (tour == null) return NotFound("Tour không tồn tại.");
        return Ok(tour);
    }

    [HttpPost]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> CreateTour(CreateTourDto request)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var tour = await _tourService.CreateTourAsync(userId, request);
            return CreatedAtAction(nameof(GetTour), new { id = tour.Id }, tour);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> UpdateTour(int id, CreateTourDto request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _tourService.UpdateTourAsync(userId, id, request);
        if (!result) return NotFound("Tour không tồn tại hoặc bạn không có quyền.");
        return Ok("Cập nhật tour thành công.");
    }

    [HttpDelete("{id}")]
[Authorize(Roles = "Provider")]
public async Task<IActionResult> DeleteTour(int id)
{
    try
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var (success, message) = await _tourService.DeleteTourAsync(userId, id);

        if (!success) return BadRequest(message);

        return Ok(message);
    }
    catch (UnauthorizedAccessException) // ← thêm
    {
        return Forbid();
    }
}

    [HttpGet("my-tours")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> GetMyTours()
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var tours = await _tourService.GetMyToursAsync(userId);
            return Ok(tours);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}