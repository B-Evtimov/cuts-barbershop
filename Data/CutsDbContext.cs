using Cuts.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Cuts.Api.Data;

public class CutsDbContext : DbContext
{
    public CutsDbContext(DbContextOptions<CutsDbContext> options) : base(options) { }

    public DbSet<Barber> Barbers => Set<Barber>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BlockedDay> BlockedDays => Set<BlockedDay>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Barber>(e =>
        {
            e.HasKey(b => b.Id);
            e.Property(b => b.Name).IsRequired().HasMaxLength(100);
            e.Property(b => b.Username).IsRequired().HasMaxLength(50);
            e.Property(b => b.PasswordHash).IsRequired();
            e.HasIndex(b => b.Username).IsUnique();
        });

        modelBuilder.Entity<Booking>(e =>
        {
            e.HasKey(b => b.Id);
            e.Property(b => b.Service).IsRequired().HasMaxLength(100);
            e.Property(b => b.ClientName).IsRequired().HasMaxLength(100);
            e.Property(b => b.ClientPhone).IsRequired().HasMaxLength(20);
            e.Property(b => b.ClientEmail).IsRequired().HasMaxLength(200);
            e.HasIndex(b => new { b.BarberId, b.Date, b.Time }).IsUnique();
            e.HasOne(b => b.Barber)
             .WithMany(bar => bar.Bookings)
             .HasForeignKey(b => b.BarberId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BlockedDay>(e =>
        {
            e.HasKey(b => b.Id);
            e.HasIndex(b => new { b.BarberId, b.Date }).IsUnique();
            e.HasOne(b => b.Barber)
             .WithMany(bar => bar.BlockedDays)
             .HasForeignKey(b => b.BarberId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
