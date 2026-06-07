using CourseWork.Data.Repositories;

namespace CourseWork.Data.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    ITourRepository Tours { get; }
    IUserRepository Users { get; }
    ITourBookingRepository TourBookings { get; }
    ITicketBookingRepository TicketBookings { get; }
    ICountryRepository Countries { get; }
    ITransportRepository Transports { get; }
    IHotelRoomRepository HotelRooms { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
