namespace Cuts.Api.Models;

public class Barber
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string AvatarEmoji { get; set; } = "💈";
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int SatisfactionPercent { get; set; } = 98;
    public int TotalClients { get; set; } = 0;
    public double Rating { get; set; } = 5.0;
    public List<Booking> Bookings { get; set; } = new();
    public List<BlockedDay> BlockedDays { get; set; } = new();
}

public class Booking
{
    public int Id { get; set; }
    public int BarberId { get; set; }
    public Barber Barber { get; set; } = null!;
    public string Service { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string ClientPhone { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;
    public BookingStatus Status { get; set; } = BookingStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum BookingStatus
{
    Active = 0,
    Cancelled = 1
}

public class BlockedDay
{
    public int Id { get; set; }
    public int BarberId { get; set; }
    public Barber Barber { get; set; } = null!;
    public DateOnly Date { get; set; }
    public string? Reason { get; set; }
}
