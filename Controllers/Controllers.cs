using Cuts.Api.Data;
using Cuts.Api.DTOs;
using Cuts.Api.Models;
using Cuts.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cuts.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BarbersController : ControllerBase
{
    private readonly CutsDbContext _db;
    public BarbersController(CutsDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<BarberResponse>>> GetAll()
    {
        var barbers = await _db.Barbers
            .Select(b => new BarberResponse(
                b.Id, b.Name, b.Title, b.AvatarEmoji,
                b.SatisfactionPercent, b.TotalClients, b.Rating))
            .ToListAsync();

        return Ok(barbers);
    }
}

[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _schedule;
    public ScheduleController(IScheduleService schedule) => _schedule = schedule;

    [HttpGet("{barberId}/{date}")]
    public async Task<ActionResult<List<TimeSlotResponse>>> GetSlots(int barberId, DateOnly date)
    {
        var slots = await _schedule.GetSlotsAsync(barberId, date, isAdmin: false);
        return Ok(slots);
    }
}

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly CutsDbContext _db;
    private readonly IScheduleService _schedule;
    private readonly INotificationService _notifications;

    public BookingsController(CutsDbContext db, IScheduleService schedule, INotificationService notifications)
    {
        _db = db;
        _schedule = schedule;
        _notifications = notifications;
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponse>> Create([FromBody] CreateBookingRequest req)
    {
        var barber = await _db.Barbers.FindAsync(req.BarberId);
        if (barber == null)
            return NotFound(new ApiError("Barber not found."));

        if (!await _schedule.IsSlotAvailableAsync(req.BarberId, req.Date, req.Time))
            return Conflict(new ApiError("This time slot is already taken. Please choose another."));

        var booking = new Booking
        {
            BarberId = req.BarberId,
            Service = req.Service,
            Date = req.Date,
            Time = req.Time,
            ClientName = req.ClientName,
            ClientPhone = req.ClientPhone,
            ClientEmail = req.ClientEmail,
            Status = BookingStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        booking.Barber = barber;
        _ = Task.Run(() => _notifications.SendBookingConfirmationAsync(booking));

        return CreatedAtAction(nameof(Create), new { id = booking.Id }, MapToResponse(booking));
    }

    private static BookingResponse MapToResponse(Booking b) => new(
        b.Id, b.BarberId, b.Barber?.Name ?? "—",
        b.Service, b.Date, b.Time,
        b.ClientName, b.ClientPhone, b.ClientEmail,
        b.Status.ToString(), b.CreatedAt
    );
}

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly CutsDbContext _db;
    private readonly IScheduleService _schedule;
    private readonly INotificationService _notifications;

    public AdminController(CutsDbContext db, IScheduleService schedule, INotificationService notifications)
    {
        _db = db;
        _schedule = schedule;
        _notifications = notifications;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req)
    {
        var barber = await _db.Barbers.FirstOrDefaultAsync(b => b.Username == req.Username);

        if (barber == null || !BCrypt.Net.BCrypt.Verify(req.Password, barber.PasswordHash))
            return Unauthorized(new ApiError("Invalid username or password."));

        return Ok(new LoginResponse(barber.Id, barber.Name, barber.Id.ToString()));
    }

    [HttpGet("{barberId}/schedule/{date}")]
    public async Task<ActionResult<List<TimeSlotResponse>>> GetSchedule(int barberId, DateOnly date)
    {
        if (!await _db.Barbers.AnyAsync(b => b.Id == barberId))
            return NotFound(new ApiError("Barber not found."));

        var slots = await _schedule.GetSlotsAsync(barberId, date, isAdmin: true);
        return Ok(slots);
    }

    [HttpGet("bookings/{id}")]
    public async Task<ActionResult<BookingResponse>> GetBooking(int id)
    {
        var booking = await _db.Bookings
            .Include(b => b.Barber)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
            return NotFound(new ApiError("Booking not found."));

        return Ok(new BookingResponse(
            booking.Id, booking.BarberId, booking.Barber.Name,
            booking.Service, booking.Date, booking.Time,
            booking.ClientName, booking.ClientPhone, booking.ClientEmail,
            booking.Status.ToString(), booking.CreatedAt
        ));
    }

    [HttpDelete("bookings/{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await _db.Bookings
            .Include(b => b.Barber)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
            return NotFound(new ApiError("Booking not found."));

        if (booking.Status == BookingStatus.Cancelled)
            return BadRequest(new ApiError("Booking is already cancelled."));

        booking.Status = BookingStatus.Cancelled;
        await _db.SaveChangesAsync();

        _ = Task.Run(() => _notifications.SendBookingCancellationAsync(booking));

        return NoContent();
    }

    [HttpPost("{barberId}/block")]
    public async Task<ActionResult<BlockedDayResponse>> BlockDay(int barberId, [FromBody] BlockDayRequest req)
    {
        var existing = await _db.BlockedDays
            .FirstOrDefaultAsync(d => d.BarberId == barberId && d.Date == req.Date);

        if (existing != null)
            return Conflict(new ApiError("This day is already blocked."));

        var blocked = new BlockedDay { BarberId = barberId, Date = req.Date, Reason = req.Reason };
        _db.BlockedDays.Add(blocked);
        await _db.SaveChangesAsync();

        return Ok(new BlockedDayResponse(blocked.Id, blocked.Date, blocked.Reason));
    }

    [HttpDelete("{barberId}/block/{date}")]
    public async Task<IActionResult> UnblockDay(int barberId, DateOnly date)
    {
        var blocked = await _db.BlockedDays
            .FirstOrDefaultAsync(d => d.BarberId == barberId && d.Date == date);

        if (blocked == null)
            return NotFound(new ApiError("This day is not blocked."));

        _db.BlockedDays.Remove(blocked);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{barberId}/blocked-days")]
    public async Task<ActionResult<List<BlockedDayResponse>>> GetBlockedDays(int barberId)
    {
        var days = await _db.BlockedDays
            .Where(d => d.BarberId == barberId)
            .OrderBy(d => d.Date)
            .Select(d => new BlockedDayResponse(d.Id, d.Date, d.Reason))
            .ToListAsync();

        return Ok(days);
    }

    [HttpGet("{barberId}/bookings")]
    public async Task<ActionResult<List<BookingResponse>>> GetBookings(int barberId, [FromQuery] DateOnly? date)
    {
        var query = _db.Bookings
            .Include(b => b.Barber)
            .Where(b => b.BarberId == barberId && b.Status == BookingStatus.Active);

        if (date.HasValue)
            query = query.Where(b => b.Date == date.Value);

        var bookings = await query
            .OrderBy(b => b.Date).ThenBy(b => b.Time)
            .Select(b => new BookingResponse(
                b.Id, b.BarberId, b.Barber.Name,
                b.Service, b.Date, b.Time,
                b.ClientName, b.ClientPhone, b.ClientEmail,
                b.Status.ToString(), b.CreatedAt))
            .ToListAsync();

        return Ok(bookings);
    }
}
