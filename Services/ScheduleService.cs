using Cuts.Api.Data;
using Cuts.Api.DTOs;
using Cuts.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Cuts.Api.Services;

public interface IScheduleService
{
    Task<List<TimeSlotResponse>> GetSlotsAsync(int barberId, DateOnly date, bool isAdmin = false);
    Task<bool> IsSlotAvailableAsync(int barberId, DateOnly date, TimeOnly time);
}

public class ScheduleService : IScheduleService
{
    private readonly CutsDbContext _db;

    private static readonly TimeOnly WorkStart = new(9, 0);
    private static readonly TimeOnly WorkEnd = new(17, 30);
    private static readonly TimeSpan SlotDuration = TimeSpan.FromMinutes(30);

    public ScheduleService(CutsDbContext db)
    {
        _db = db;
    }

    public async Task<List<TimeSlotResponse>> GetSlotsAsync(int barberId, DateOnly date, bool isAdmin = false)
    {
        var bookingsOnDate = await _db.Bookings
            .Where(b => b.BarberId == barberId && b.Date == date && b.Status == BookingStatus.Active)
            .ToListAsync();

        var isBlocked = await _db.BlockedDays
            .AnyAsync(d => d.BarberId == barberId && d.Date == date);

        var slots = new List<TimeSlotResponse>();
        var current = WorkStart;

        while (current <= WorkEnd)
        {
            if (isBlocked)
            {
                slots.Add(new TimeSlotResponse(current, IsAvailable: false));
            }
            else
            {
                var booking = bookingsOnDate.FirstOrDefault(b => b.Time == current);
                slots.Add(new TimeSlotResponse(
                    Time: current,
                    IsAvailable: booking == null,
                    BookingId: (isAdmin && booking != null) ? booking.Id : null
                ));
            }

            current = current.Add(SlotDuration);
        }

        return slots;
    }

    public async Task<bool> IsSlotAvailableAsync(int barberId, DateOnly date, TimeOnly time)
    {
        if (await _db.BlockedDays.AnyAsync(d => d.BarberId == barberId && d.Date == date))
            return false;

        if (await _db.Bookings.AnyAsync(b =>
                b.BarberId == barberId &&
                b.Date == date &&
                b.Time == time &&
                b.Status == BookingStatus.Active))
            return false;

        if (time < WorkStart || time > WorkEnd)
            return false;

        return true;
    }
}
