using CourseWork.Business.DTOs;
using CourseWork.Business.Exceptions;
using CourseWork.Business.Services;
using CourseWork.Data.Entities;
using CourseWork.Data.Enums;
using CourseWork.Data.Repositories;
using CourseWork.Data.UnitOfWork;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CourseWork.Business.Tests.Services;

public class TourServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ITourRepository> _tourRepositoryMock = new();
    private readonly Mock<ICountryRepository> _countryRepositoryMock = new();
    private readonly TourService _sut;

    public TourServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.Tours).Returns(_tourRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Countries).Returns(_countryRepositoryMock.Object);
        _sut = new TourService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMappedTours()
    {
        // Arrange
        var tour = CreateSampleTour();
        _tourRepositoryMock
            .Setup(r => r.SearchAsync(null, true, null, null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tour> { tour });

        // Act
        var result = await _sut.SearchAsync(new TourSearchFilterDto(null, true, null, null, null, null, null));

        // Assert
        result.Should().HaveCount(1);
        result[0].Title.Should().Be("Test Tour");
        result[0].CountryName.Should().Be("Україна");
    }

    [Fact]
    public async Task GetByIdAsync_WhenTourNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _tourRepositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act
        var act = () => _sut.GetByIdAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_WhenCallerIsGuest_ThrowsUnauthorizedException()
    {
        // Arrange
        var dto = new CreateTourDto("T", "D", TourType.Beach, 1, null, 1000, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(5), false, 10);

        // Act
        var act = () => _sut.CreateAsync(dto, UserRole.Registered);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedBusinessException>();
    }

    [Fact]
    public async Task CreateAsync_WhenDatesInvalid_ThrowsValidationException()
    {
        // Arrange
        var dto = new CreateTourDto("T", "D", TourType.Beach, 1, null, 1000, DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(1), false, 10);

        // Act
        var act = () => _sut.CreateAsync(dto, UserRole.Manager);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateAsync_WhenValid_CreatesTour()
    {
        // Arrange
        var dto = new CreateTourDto("New Tour", "Desc", TourType.Excursion, 1, null, 5000, DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddDays(15), true, 8);
        _countryRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(new Country { Id = 1, Name = "Україна" });
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _tourRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => CreateSampleTour(id));

        // Act
        var result = await _sut.CreateAsync(dto, UserRole.Manager);

        // Assert
        result.Title.Should().Be("Test Tour");
        _tourRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_SetsTourInactive()
    {
        // Arrange
        var tour = CreateSampleTour();
        _tourRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        // Act
        await _sut.DeleteAsync(1, UserRole.Administrator);

        // Assert
        tour.IsActive.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Tour CreateSampleTour(int id = 1) => new()
    {
        Id = id,
        Title = "Test Tour",
        Description = "Description",
        Type = TourType.Beach,
        Country = new Country { Name = "Україна" },
        Price = 10000,
        StartDate = DateTime.UtcNow.AddDays(1),
        EndDate = DateTime.UtcNow.AddDays(7),
        AvailablePlaces = 5,
        IsHot = true,
        IsActive = true
    };
}

public class BookingServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<ITourRepository> _tourRepositoryMock = new();
    private readonly Mock<ITourBookingRepository> _bookingRepositoryMock = new();
    private readonly BookingService _sut;

    public BookingServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Tours).Returns(_tourRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.TourBookings).Returns(_bookingRepositoryMock.Object);
        _sut = new BookingService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task BookTourAsync_WhenGuest_ThrowsUnauthorizedException()
    {
        // Arrange
        var dto = new CreateTourBookingDto(1, 1);

        // Act
        var act = () => _sut.BookTourAsync(1, dto, UserRole.Guest);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedBusinessException>();
    }

    [Fact]
    public async Task BookTourAsync_WhenNotEnoughPlaces_ThrowsInsufficientAvailabilityException()
    {
        // Arrange
        var user = new User { Id = 1, Role = UserRole.Registered };
        var tour = new Tour { Id = 1, IsActive = true, AvailablePlaces = 1, Price = 1000, Title = "T" };
        _userRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _tourRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        // Act
        var act = () => _sut.BookTourAsync(1, new CreateTourBookingDto(1, 2), UserRole.Registered);

        // Assert
        await act.Should().ThrowAsync<InsufficientAvailabilityException>();
    }

    [Fact]
    public async Task BookTourAsync_WhenValid_ReducesAvailablePlaces()
    {
        // Arrange
        var user = new User { Id = 1 };
        var tour = new Tour { Id = 1, IsActive = true, AvailablePlaces = 5, Price = 2000, Title = "Sea Tour" };
        _userRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _tourRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.BookTourAsync(1, new CreateTourBookingDto(1, 2), UserRole.Registered);

        // Assert
        tour.AvailablePlaces.Should().Be(3);
        result.TotalPrice.Should().Be(4000);
        result.PlacesCount.Should().Be(2);
    }

    [Fact]
    public async Task CancelBookingAsync_WhenAlreadyCancelled_ThrowsValidationException()
    {
        // Arrange
        var booking = new TourBooking { Id = 1, UserId = 1, TourId = 1, Status = BookingStatus.Cancelled, PlacesCount = 1 };
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        // Act
        var act = () => _sut.CancelBookingAsync(1, 1, UserRole.Registered);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}

public class TicketBookingServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<ITransportRepository> _transportRepositoryMock = new();
    private readonly Mock<IHotelRoomRepository> _hotelRoomRepositoryMock = new();
    private readonly Mock<ITicketBookingRepository> _ticketBookingRepositoryMock = new();
    private readonly TicketBookingService _sut;

    public TicketBookingServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Transports).Returns(_transportRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.HotelRooms).Returns(_hotelRoomRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.TicketBookings).Returns(_ticketBookingRepositoryMock.Object);
        _sut = new TicketBookingService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task BookTransportAsync_WhenValid_UpdatesSeats()
    {
        // Arrange
        var user = new User { Id = 1 };
        var transport = new Transport { Id = 1, Name = "Bus", Price = 500, AvailableSeats = 10 };
        _userRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _transportRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(transport);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.BookTransportAsync(1, new CreateTransportTicketBookingDto(1, 2), UserRole.Registered);

        // Assert
        transport.AvailableSeats.Should().Be(8);
        result.TotalPrice.Should().Be(1000);
    }

    [Fact]
    public async Task BookHotelRoomAsync_WhenRoomUnavailable_ThrowsInsufficientAvailabilityException()
    {
        // Arrange
        var user = new User { Id = 1 };
        var room = new HotelRoom { Id = 1, IsAvailable = false, Hotel = new Hotel { Name = "H" }, RoomNumber = "1", PricePerNight = 1000 };
        _userRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _hotelRoomRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(room);

        // Act
        var act = () => _sut.BookHotelRoomAsync(
            1,
            new CreateHotelRoomBookingDto(1, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3)),
            UserRole.Registered);

        // Assert
        await act.Should().ThrowAsync<InsufficientAvailabilityException>();
    }
}

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepositoryMock.Object);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Jwt:Key"] = "CourseWorkSecretKeyForJwtTokenGeneration2024!" })
            .Build();
        _sut = new UserService(_unitOfWorkMock.Object, config);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailExists_ThrowsValidationException()
    {
        // Arrange
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Email = "test@test.com" });

        // Act
        var act = () => _sut.RegisterAsync(new RegisterUserDto("test@test.com", "password1", "A", "B"));

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task RegisterAsync_WhenValid_CreatesUser()
    {
        // Arrange
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.RegisterAsync(new RegisterUserDto("new@test.com", "password1", "Ivan", "Petrov"));

        // Assert
        result.Email.Should().Be("new@test.com");
        result.Token.Should().NotBeNullOrEmpty();
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordInvalid_ThrowsUnauthorizedException()
    {
        // Arrange
        var user = new User
        {
            Email = "user@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct")
        };
        _userRepositoryMock.Setup(r => r.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var act = () => _sut.LoginAsync(new LoginDto("user@test.com", "wrong"));

        // Assert
        await act.Should().ThrowAsync<UnauthorizedBusinessException>();
    }
}
