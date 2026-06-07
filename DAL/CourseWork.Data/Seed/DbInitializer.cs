using CourseWork.Data.Context;
using CourseWork.Data.Entities;
using CourseWork.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CourseWork.Data.Seed;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.MigrateAsync();

        if (await context.Users.AnyAsync())
        {
            return;
        }

        var ukraine = new Country { Name = "Україна" };
        var turkey = new Country { Name = "Туреччина" };
        var spain = new Country { Name = "Іспанія" };

        var kyivRegion = new Region { Name = "Київ", Country = ukraine };
        var lvivRegion = new Region { Name = "Львів", Country = ukraine };
        var antalyaRegion = new Region { Name = "Анталія", Country = turkey };
        var barcelonaRegion = new Region { Name = "Барселона", Country = spain };

        var admin = new User
        {
            Email = "admin@agency.ua",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            FirstName = "Адмін",
            LastName = "Системи",
            Role = UserRole.Administrator
        };

        var manager = new User
        {
            Email = "manager@agency.ua",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"),
            FirstName = "Олена",
            LastName = "Менеджер",
            Role = UserRole.Manager
        };

        var user = new User
        {
            Email = "user@agency.ua",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
            FirstName = "Іван",
            LastName = "Петренко",
            Role = UserRole.Registered
        };

        var tours = new List<Tour>
        {
            new()
            {
                Title = "Екскурсія по Львову",
                Description = "3-денна екскурсійна програма старим містом.",
                Type = TourType.Excursion,
                Country = ukraine,
                Region = lvivRegion,
                Price = 4500,
                StartDate = DateTime.UtcNow.AddDays(14),
                EndDate = DateTime.UtcNow.AddDays(17),
                IsHot = false,
                AvailablePlaces = 20
            },
            new()
            {
                Title = "Гарячий тур до Анталії",
                Description = "Пляжний відпочинок all inclusive.",
                Type = TourType.Beach,
                Country = turkey,
                Region = antalyaRegion,
                Price = 18900,
                StartDate = DateTime.UtcNow.AddDays(7),
                EndDate = DateTime.UtcNow.AddDays(14),
                IsHot = true,
                AvailablePlaces = 15
            },
            new()
            {
                Title = "Барcelona Cultural Week",
                Description = "Культурна програма з відвідуванням музеїв Гaudí.",
                Type = TourType.Cultural,
                Country = spain,
                Region = barcelonaRegion,
                Price = 22500,
                StartDate = DateTime.UtcNow.AddDays(30),
                EndDate = DateTime.UtcNow.AddDays(37),
                IsHot = true,
                AvailablePlaces = 10
            }
        };

        var hotel = new Hotel
        {
            Name = "Grand Antalya Resort",
            Region = antalyaRegion,
            StarRating = 5,
            Rooms =
            [
                new HotelRoom { RoomNumber = "101", PricePerNight = 3500, Capacity = 2 },
                new HotelRoom { RoomNumber = "205", PricePerNight = 5200, Capacity = 3 }
            ]
        };

        var transports = new List<Transport>
        {
            new()
            {
                Name = "SkyWings UA-101",
                Type = TransportType.Air,
                Route = "Київ — Анталія",
                Price = 6800,
                DepartureTime = DateTime.UtcNow.AddDays(6),
                AvailableSeats = 120
            },
            new()
            {
                Name = "EuroRail Express",
                Type = TransportType.Train,
                Route = "Львів — Барселона",
                Price = 4200,
                DepartureTime = DateTime.UtcNow.AddDays(28),
                AvailableSeats = 80
            }
        };

        context.Countries.AddRange(ukraine, turkey, spain);
        context.Users.AddRange(admin, manager, user);
        context.Tours.AddRange(tours);
        context.Hotels.Add(hotel);
        context.Transports.AddRange(transports);

        await context.SaveChangesAsync();
    }
}
