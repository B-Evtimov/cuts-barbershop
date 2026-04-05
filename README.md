# ✂️ CUTS Barbershop — Booking System

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![SQLite](https://img.shields.io/badge/Database-SQLite-003B57?style=flat-square&logo=sqlite)
![License](https://img.shields.io/badge/license-MIT-green?style=flat-square)

A full-stack barbershop appointment booking system built with **ASP.NET Core 8** and **SQLite**. Press Play in Visual Studio and everything works — no external services required.

## Features

**Client Site** — 4-step booking wizard (Service → Barber → Time Slot → Contact Info) with animated dark UI and real-time slot availability.

**Admin Panel** — per-barber login, daily schedule timeline, booking detail modal with two-step delete confirmation, and day blocking.

**API** — RESTful ASP.NET Core Web API with Swagger UI at `/swagger`.

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core 8 Web API |
| Database | SQLite via Entity Framework Core 8 |
| Auth | BCrypt password hashing |
| Frontend | Vanilla HTML / CSS / JavaScript |

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 or VS Code

### Run

```bash
git clone https://github.com/YOUR_USERNAME/cuts-barbershop.git
cd cuts-barbershop
dotnet run
```

Or press **Play (F5)** in Visual Studio.

The database (`cuts.db`) is created automatically on first run with two seed barbers.

Open `http://localhost:5000` — client booking site loads immediately.

**Test admin accounts:** `ivan / 1234` &nbsp;|&nbsp; `martin / 1234`

**Swagger UI:** `http://localhost:5000/swagger`

## Project Structure

```
cuts-barbershop/
├── Controllers/Controllers.cs      # All API endpoints
├── Data/CutsDbContext.cs           # EF Core DbContext + seed data
├── DTOs/DTOs.cs                    # Request & response models
├── Models/Models.cs                # Barber, Booking, BlockedDay
├── Services/
│   ├── NotificationService.cs      # Logs notifications to console
│   └── ScheduleService.cs          # Time slot business logic
├── wwwroot/
│   ├── index.html                  # Client booking site
│   └── admin.html                  # Barber admin panel
├── Program.cs                      # App startup
└── appsettings.json
```

## API Endpoints

| Method | Route | Description |
|---|---|---|
| GET | `/api/barbers` | List all barbers |
| GET | `/api/schedule/{barberId}/{date}` | Available slots for a date |
| POST | `/api/bookings` | Create a booking |
| POST | `/api/admin/login` | Barber login |
| GET | `/api/admin/{id}/schedule/{date}` | Admin schedule view |
| GET | `/api/admin/bookings/{id}` | Booking details |
| DELETE | `/api/admin/bookings/{id}` | Cancel a booking |
| POST | `/api/admin/{id}/block` | Block a day |
| DELETE | `/api/admin/{id}/block/{date}` | Unblock a day |
| GET | `/api/admin/{id}/blocked-days` | List blocked days |

## Azure Deployment

This project is structured to be deployed to **Azure App Service** with minimal changes:

- Swap SQLite → **Azure SQL Database** (change one line in `Program.cs`)
- Add **Azure Key Vault** for secrets
- Add **Azure Communication Services** for real email/SMS

See `AZURE-GUIDE.md` for step-by-step instructions.

## License

MIT
