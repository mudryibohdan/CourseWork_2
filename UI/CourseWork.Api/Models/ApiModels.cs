using CourseWork.Data.Enums;

namespace CourseWork.Api.Models;

public class TourResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TourType Type { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public string? RegionName { get; set; }
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsHot { get; set; }
    public int AvailablePlaces { get; set; }
}

public class CreateTourRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TourType Type { get; set; }
    public int CountryId { get; set; }
    public int? RegionId { get; set; }
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsHot { get; set; }
    public int AvailablePlaces { get; set; }
}

public class UpdateTourRequest : CreateTourRequest
{
    public bool IsActive { get; set; } = true;
}

public class TourSearchRequest
{
    public string? SearchTerm { get; set; }
    public bool? IsHot { get; set; }
    public TourType? Type { get; set; }
    public int? CountryId { get; set; }
    public int? RegionId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponse
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string Token { get; set; } = string.Empty;
}

public class CreateTourBookingRequest
{
    public int TourId { get; set; }
    public int PlacesCount { get; set; }
}

public class CreateTransportBookingRequest
{
    public int TransportId { get; set; }
    public int Quantity { get; set; }
}

public class CreateHotelRoomBookingRequest
{
    public int HotelRoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}

public class TourBookingResponse
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public string TourTitle { get; set; } = string.Empty;
    public int PlacesCount { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime BookingDate { get; set; }
}

public class TicketBookingResponse
{
    public int Id { get; set; }
    public TicketBookingType BookingType { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime BookingDate { get; set; }
}

public class CountryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<RegionResponse> Regions { get; set; } = [];
}

public class RegionResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CountryId { get; set; }
}

public class TransportResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TransportType Type { get; set; }
    public string Route { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime DepartureTime { get; set; }
    public int AvailableSeats { get; set; }
}

public class HotelRoomResponse
{
    public int Id { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public bool IsAvailable { get; set; }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
}
