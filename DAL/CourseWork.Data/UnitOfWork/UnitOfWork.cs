using CourseWork.Data.Context;
using CourseWork.Data.Repositories;

namespace CourseWork.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Tours = new TourRepository(_context);
        Users = new UserRepository(_context);
        TourBookings = new TourBookingRepository(_context);
        TicketBookings = new TicketBookingRepository(_context);
        Countries = new CountryRepository(_context);
        Transports = new TransportRepository(_context);
        HotelRooms = new HotelRoomRepository(_context);
    }

    public ITourRepository Tours { get; }
    public IUserRepository Users { get; }
    public ITourBookingRepository TourBookings { get; }
    public ITicketBookingRepository TicketBookings { get; }
    public ICountryRepository Countries { get; }
    public ITransportRepository Transports { get; }
    public IHotelRoomRepository HotelRooms { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
