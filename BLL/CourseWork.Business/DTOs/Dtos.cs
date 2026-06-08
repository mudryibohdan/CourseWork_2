using CourseWork.Data.Enums;

namespace CourseWork.Business.DTOs;

public record TourDto(
    int Id,
    string Title,
    string Description,
    TourType Type,
    string CountryName,
    string? RegionName,
    decimal Price,
    DateTime StartDate,
    DateTime EndDate,
    bool IsHot,
    int AvailablePlaces);

public record CreateTourDto(
    string Title,
    string Description,
    TourType Type,
    int CountryId,
    int? RegionId,
    decimal Price,
    DateTime StartDate,
    DateTime EndDate,
    bool IsHot,
    int AvailablePlaces);

public record UpdateTourDto(
    string Title,
    string Description,
    TourType Type,
    int CountryId,
    int? RegionId,
    decimal Price,
    DateTime StartDate,
    DateTime EndDate,
    bool IsHot,
    int AvailablePlaces,
    bool IsActive);

public record TourSearchFilterDto(
    string? SearchTerm,
    bool? IsHot,
    TourType? Type,
    int? CountryId,
    int? RegionId,
    decimal? MinPrice,
    decimal? MaxPrice);

public record UserDto(
    int Id,
    string Email,
    string FirstName,
    string LastName,
    UserRole Role);

public record RegisterUserDto(string Email, string Password, string FirstName, string LastName);

public record LoginDto(string Email, string Password);

public record AuthResultDto(int UserId, string Email, string FullName, UserRole Role, string Token);

public record TourBookingDto(
    int Id,
    int TourId,
    string TourTitle,
    int PlacesCount,
    decimal TotalPrice,
    BookingStatus Status,
    DateTime BookingDate);

public record CreateTourBookingDto(int TourId, int PlacesCount);

public record TicketBookingDto(
    int Id,
    TicketBookingType BookingType,
    string ItemName,
    int Quantity,
    decimal TotalPrice,
    BookingStatus Status,
    DateTime BookingDate);

public record CreateTransportTicketBookingDto(int TransportId, int Quantity);

public record CreateHotelRoomBookingDto(int HotelRoomId, DateTime CheckInDate, DateTime CheckOutDate);

public record CountryDto(int Id, string Name, IReadOnlyList<RegionDto> Regions);

public record RegionDto(int Id, string Name, int CountryId);

public record TransportDto(
    int Id,
    string Name,
    TransportType Type,
    string Route,
    decimal Price,
    DateTime DepartureTime,
    int AvailableSeats);

public record HotelRoomDto(
    int Id,
    string HotelName,
    string RoomNumber,
    decimal PricePerNight,
    int Capacity,
    bool IsAvailable);
