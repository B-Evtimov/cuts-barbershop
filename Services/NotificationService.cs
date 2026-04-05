using Cuts.Api.Models;

namespace Cuts.Api.Services;

public interface INotificationService
{
    Task SendBookingConfirmationAsync(Booking booking);
    Task SendBookingCancellationAsync(Booking booking);
}

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendBookingConfirmationAsync(Booking booking)
    {
        _logger.LogInformation(
            "[EMAIL] Booking confirmed → {Email} | {Service} with {Barber} on {Date} at {Time}",
            booking.ClientEmail,
            booking.Service,
            booking.Barber?.Name ?? "—",
            booking.Date.ToString("dd MMM yyyy"),
            booking.Time.ToString("HH:mm")
        );

        _logger.LogInformation(
            "[SMS] → {Phone} | Booking confirmed: {Service} on {Date} at {Time}",
            booking.ClientPhone,
            booking.Service,
            booking.Date.ToString("dd MMM yyyy"),
            booking.Time.ToString("HH:mm")
        );

        return Task.CompletedTask;
    }

    public Task SendBookingCancellationAsync(Booking booking)
    {
        _logger.LogInformation(
            "[EMAIL] Booking cancelled → {Email} | {Date} at {Time}",
            booking.ClientEmail,
            booking.Date.ToString("dd MMM yyyy"),
            booking.Time.ToString("HH:mm")
        );

        _logger.LogInformation(
            "[SMS] → {Phone} | Booking cancelled: {Date} at {Time}",
            booking.ClientPhone,
            booking.Date.ToString("dd MMM yyyy"),
            booking.Time.ToString("HH:mm")
        );

        return Task.CompletedTask;
    }
}
