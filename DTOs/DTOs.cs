namespace Cuts.Api.DTOs;

public record CreateBookingRequest(
    int BarberId,
    string Service,
    DateOnly Date,
    TimeOnly Time,
    string ClientName,
    string ClientPhone,
    string ClientEmail
);

public record LoginRequest(
    string Username,
    string Password
);

public record BlockDayRequest(
    DateOnly Date,
    string? Reason
);

public record BarberResponse(
    int Id,
    string Name,
    string Title,
    string AvatarEmoji,
    int SatisfactionPercent,
    int TotalClients,
    double Rating
);

public record BookingResponse(
    int Id,
    int BarberId,
    string BarberName,
    string Service,
    DateOnly Date,
    TimeOnly Time,
    string ClientName,
    string ClientPhone,
    string ClientEmail,
    string Status,
    DateTime CreatedAt
);

public record TimeSlotResponse(
    TimeOnly Time,
    bool IsAvailable,
    int? BookingId = null
);

public record LoginResponse(
    int BarberId,
    string Name,
    string Token
);

public record BlockedDayResponse(
    int Id,
    DateOnly Date,
    string? Reason
);

public record ApiError(string Message);
