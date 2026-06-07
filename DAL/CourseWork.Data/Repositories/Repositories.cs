using System.Linq.Expressions;
using CourseWork.Data.Context;
using CourseWork.Data.Entities;
using CourseWork.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.Data.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(ApplicationDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public virtual void Remove(TEntity entity)
    {
        DbSet.Remove(entity);
    }
}

public class TourRepository : Repository<Tour>, ITourRepository
{
    public TourRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Tour>> SearchAsync(
        string? searchTerm,
        bool? isHot,
        TourType? type,
        int? countryId,
        int? regionId,
        decimal? minPrice,
        decimal? maxPrice,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(t => t.Country)
            .Include(t => t.Region)
            .Where(t => t.IsActive);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(t =>
                t.Title.ToLower().Contains(term) ||
                t.Description.ToLower().Contains(term) ||
                t.Country.Name.ToLower().Contains(term));
        }

        if (isHot.HasValue)
        {
            query = query.Where(t => t.IsHot == isHot.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(t => t.Type == type.Value);
        }

        if (countryId.HasValue)
        {
            query = query.Where(t => t.CountryId == countryId.Value);
        }

        if (regionId.HasValue)
        {
            query = query.Where(t => t.RegionId == regionId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(t => t.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(t => t.Price <= maxPrice.Value);
        }

        return await query.OrderBy(t => t.StartDate).ToListAsync(cancellationToken);
    }

    public override async Task<Tour?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(t => t.Country)
            .Include(t => t.Region)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }
}

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}

public class TourBookingRepository : Repository<TourBooking>, ITourBookingRepository
{
    public TourBookingRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<TourBooking>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Tour)
            .ThenInclude(t => t.Country)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync(cancellationToken);
    }
}

public class TicketBookingRepository : Repository<TicketBooking>, ITicketBookingRepository
{
    public TicketBookingRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<TicketBooking>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Transport)
            .Include(b => b.HotelRoom)
            .ThenInclude(r => r!.Hotel)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync(cancellationToken);
    }
}

public class CountryRepository : Repository<Country>, ICountryRepository
{
    public CountryRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Country>> GetAllWithRegionsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.Regions)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }
}

public class TransportRepository : Repository<Transport>, ITransportRepository
{
    public TransportRepository(ApplicationDbContext context) : base(context) { }
}

public class HotelRoomRepository : Repository<HotelRoom>, IHotelRoomRepository
{
    public HotelRoomRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<HotelRoom?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Hotel)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<HotelRoom>> GetAvailableAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Hotel)
            .ThenInclude(h => h.Region)
            .Where(r => r.IsAvailable)
            .ToListAsync(cancellationToken);
    }
}
