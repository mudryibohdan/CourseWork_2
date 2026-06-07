using System.Linq.Expressions;
using CourseWork.Data.Entities;
using CourseWork.Data.Enums;

namespace CourseWork.Data.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}

public interface ITourRepository : IRepository<Tour>
{
    Task<IReadOnlyList<Tour>> SearchAsync(
        string? searchTerm,
        bool? isHot,
        TourType? type,
        int? countryId,
        int? regionId,
        decimal? minPrice,
        decimal? maxPrice,
        CancellationToken cancellationToken = default);
}

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}

public interface ITourBookingRepository : IRepository<TourBooking>
{
    Task<IReadOnlyList<TourBooking>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}

public interface ITicketBookingRepository : IRepository<TicketBooking>
{
    Task<IReadOnlyList<TicketBooking>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}

public interface ICountryRepository : IRepository<Country>
{
    Task<IReadOnlyList<Country>> GetAllWithRegionsAsync(CancellationToken cancellationToken = default);
}

public interface ITransportRepository : IRepository<Transport> { }

public interface IHotelRoomRepository : IRepository<HotelRoom>
{
    Task<IReadOnlyList<HotelRoom>> GetAvailableAsync(CancellationToken cancellationToken = default);
}
