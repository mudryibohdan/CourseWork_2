using CourseWork.Business.DTOs;
using CourseWork.Data.Enums;

namespace CourseWork.Business.Services;

public interface ITourService
{
    Task<IReadOnlyList<TourDto>> SearchAsync(TourSearchFilterDto filter, CancellationToken cancellationToken = default);
    Task<TourDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TourDto> CreateAsync(CreateTourDto dto, UserRole callerRole, CancellationToken cancellationToken = default);
    Task<TourDto> UpdateAsync(int id, UpdateTourDto dto, UserRole callerRole, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, UserRole callerRole, CancellationToken cancellationToken = default);
}

public interface IBookingService
{
    Task<TourBookingDto> BookTourAsync(int userId, CreateTourBookingDto dto, UserRole callerRole, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TourBookingDto>> GetUserBookingsAsync(int userId, CancellationToken cancellationToken = default);
    Task CancelBookingAsync(int bookingId, int userId, UserRole callerRole, CancellationToken cancellationToken = default);
}

public interface ITicketBookingService
{
    Task<TicketBookingDto> BookTransportAsync(int userId, CreateTransportTicketBookingDto dto, UserRole callerRole, CancellationToken cancellationToken = default);
    Task<TicketBookingDto> BookHotelRoomAsync(int userId, CreateHotelRoomBookingDto dto, UserRole callerRole, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TicketBookingDto>> GetUserBookingsAsync(int userId, CancellationToken cancellationToken = default);
}

public interface IUserService
{
    Task<AuthResultDto> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default);
    Task<AuthResultDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    Task<UserDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}

public interface IReferenceDataService
{
    Task<IReadOnlyList<CountryDto>> GetCountriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransportDto>> GetTransportsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HotelRoomDto>> GetAvailableHotelRoomsAsync(CancellationToken cancellationToken = default);
}
