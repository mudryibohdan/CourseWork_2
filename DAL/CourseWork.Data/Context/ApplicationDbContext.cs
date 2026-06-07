using CourseWork.Data.Entities;
using CourseWork.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.Data.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Region> Regions => Set<Region>();
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<HotelRoom> HotelRooms => Set<HotelRoom>();
    public DbSet<Transport> Transports => Set<Transport>();
    public DbSet<TourBooking> TourBookings => Set<TourBooking>();
    public DbSet<TicketBooking> TicketBookings => Set<TicketBooking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).HasMaxLength(256);
            entity.Property(u => u.FirstName).HasMaxLength(100);
            entity.Property(u => u.LastName).HasMaxLength(100);
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.Property(c => c.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.Property(r => r.Name).HasMaxLength(100);
            entity.HasOne(r => r.Country)
                .WithMany(c => c.Regions)
                .HasForeignKey(r => r.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Tour>(entity =>
        {
            entity.Property(t => t.Title).HasMaxLength(200);
            entity.Property(t => t.Price).HasPrecision(18, 2);
            entity.HasOne(t => t.Country)
                .WithMany(c => c.Tours)
                .HasForeignKey(t => t.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(t => t.Region)
                .WithMany(r => r.Tours)
                .HasForeignKey(t => t.RegionId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.Property(h => h.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<HotelRoom>(entity =>
        {
            entity.Property(r => r.PricePerNight).HasPrecision(18, 2);
            entity.Property(r => r.RoomNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<Transport>(entity =>
        {
            entity.Property(t => t.Price).HasPrecision(18, 2);
            entity.Property(t => t.Name).HasMaxLength(200);
            entity.Property(t => t.Route).HasMaxLength(300);
        });

        modelBuilder.Entity<TourBooking>(entity =>
        {
            entity.Property(b => b.TotalPrice).HasPrecision(18, 2);
        });

        modelBuilder.Entity<TicketBooking>(entity =>
        {
            entity.Property(b => b.TotalPrice).HasPrecision(18, 2);
        });
    }
}
