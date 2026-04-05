# ✂️ CUTS Barbershop — Online Booking System

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![SQLite](https://img.shields.io/badge/Database-SQLite-003B57?style=flat-square&logo=sqlite)
![Entity Framework](https://img.shields.io/badge/ORM-Entity_Framework_Core-512BD4?style=flat-square&logo=dotnet)
![BCrypt](https://img.shields.io/badge/Auth-BCrypt-green?style=flat-square)
![License](https://img.shields.io/badge/license-MIT-green?style=flat-square)

A full-stack barbershop appointment booking system built with **ASP.NET Core 8** and **SQLite**. Press Play in Visual Studio — everything works instantly, no additional setup required.

---

## 🖼️ Screenshots

### Home Page
![Hero](screenshots/01_hero.png)

### Step 1 — Choose a Service
![Services](screenshots/02_services.png)

### Step 2 — Choose a Barber
![Barbers](screenshots/03_barbers.png)

### Step 3 — Pick a Time Slot
![Time Slots](screenshots/04_timeslots.png)

### Step 4 — Enter Your Details
![Details](screenshots/05_details.png)

### Booking Confirmed
![Confirmed](screenshots/06_confirmed.png)

### Admin Panel — Login
![Admin Login](screenshots/07_admin_login.png)

### Admin Panel — Schedule
![Admin Schedule](screenshots/08_admin_schedule.png)

### Booking Detail Modal
![Booking Modal](screenshots/10_admin_modal.png)

### Delete Confirmation
![Delete Confirm](screenshots/11_admin_modal_confirm.png)

### Blocked Days
![Blocked Days](screenshots/09_admin_blocked.png)

---

## ✨ Features

### Client Booking Site
- **4-step booking wizard** — Service → Barber → Time Slot → Contact Details
- Real-time slot availability fetched from the database
- 30-minute slots, working hours 9:00–17:30
- Animated dark UI with CSS transitions and particle effects
- Progress bar, hover effects, and a success screen on completion

### Admin Panel
- Individual login per barber with BCrypt-hashed passwords
- Daily schedule timeline showing all bookings at a glance
- Click any booking → modal with full client details
- **Two-step delete confirmation** — asks "Are you sure?" before removing
- Block and unblock entire days to prevent new bookings
- Stats dashboard: today's bookings, total, this week, blocked days

### API
- RESTful ASP.NET Core Web API
- Swagger UI at `/swagger` to browse and test all endpoints
- Notifications logged to console (ready for email/SMS on real deployment)

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core 8 Web API |
| Database | SQLite via Entity Framework Core 8 |
| ORM | Entity Framework Core — code-first approach |
| Auth | BCrypt password hashing |
| Frontend | Vanilla HTML / CSS / JavaScript |
| Fonts | Bebas Neue + DM Sans (Google Fonts) |

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 or VS Code

### Run

```bash
git clone https://github.com/B-Evtimov/cuts-barbershop.git
cd cuts-barbershop
dotnet run
```

Or open `Cuts.Api.csproj` in Visual Studio and press **F5**.

On first run, `cuts.db` is created automatically and seeded with two barbers.

Open your browser at `http://localhost:5000`

**Test admin accounts:** `ivan / 1234` &nbsp;|&nbsp; `martin / 1234`

**Swagger UI:** `http://localhost:5000/swagger`

---

## 📁 Project Structure

```
cuts-barbershop/
├── Controllers/
│   └── Controllers.cs          # All API endpoints
├── Data/
│   └── CutsDbContext.cs        # EF Core DbContext
├── DTOs/
│   └── DTOs.cs                 # Request & response models
├── Models/
│   └── Models.cs               # Barber, Booking, BlockedDay
├── Services/
│   ├── NotificationService.cs  # Console notification logger
│   └── ScheduleService.cs      # Time slot business logic
├── wwwroot/
│   ├── index.html              # Client booking site
│   └── admin.html              # Barber admin panel
├── screenshots/                # README screenshots
├── Program.cs                  # App startup + DB seeding
├── Cuts.Api.csproj
└── appsettings.json
```

---

## 📡 API Endpoints

### Public

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/barbers` | List all barbers |
| `GET` | `/api/schedule/{barberId}/{date}` | Available slots for a date |
| `POST` | `/api/bookings` | Create a new booking |

### Admin

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/admin/login` | Barber login |
| `GET` | `/api/admin/{id}/schedule/{date}` | Schedule with booking IDs |
| `GET` | `/api/admin/bookings/{id}` | Booking details |
| `DELETE` | `/api/admin/bookings/{id}` | Cancel a booking |
| `POST` | `/api/admin/{id}/block` | Block a day |
| `DELETE` | `/api/admin/{id}/block/{date}` | Unblock a day |
| `GET` | `/api/admin/{id}/blocked-days` | List blocked days |

---

## 🗄️ Database

Three tables created automatically on first run:

- `Barbers` — barbers with hashed passwords and stats
- `Bookings` — appointments with a unique index on (barber + date + time)
- `BlockedDays` — days blocked per barber

---

## ☁️ Azure Deployment

The project is structured for easy Azure deployment:

- SQLite → **Azure SQL Database** (one line change in `Program.cs`)
- Notifications → **Azure Communication Services** (email + SMS)
- Secrets → **Azure Key Vault** with Managed Identity

---

## 📄 License

MIT
