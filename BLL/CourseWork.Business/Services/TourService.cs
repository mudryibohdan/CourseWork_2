using CourseWork.Business.DTOs;
using CourseWork.Business.Exceptions;
using CourseWork.Business.Mapping;
using CourseWork.Data.Enums;
using CourseWork.Data.UnitOfWork;

namespace CourseWork.Business.Services;

public class TourService : ITourService
{
    private readonly IUnitOfWork _unitOfWork;

    public TourService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<TourDto>> SearchAsync(TourSearchFilterDto filter, CancellationToken cancellationToken = default)
    {
        var tours = await _unitOfWork.Tours.SearchAsync(
            filter.SearchTerm,
            filter.IsHot,
            filter.Type,
            filter.CountryId,
            filter.RegionId,
            filter.MinPrice,
            filter.MaxPrice,
            cancellationToken);

        return tours.Select(EntityMapper.ToDto).ToList();
    }

    public async Task<TourDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var tour = await _unitOfWork.Tours.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Тур", id);

        return EntityMapper.ToDto(tour);
    }

    public async Task<TourDto> CreateAsync(CreateTourDto dto, UserRole callerRole, CancellationToken cancellationToken = default)
    {
        EnsureCanManageTours(callerRole);
        ValidateTourDates(dto.StartDate, dto.EndDate);
        ValidatePriceAndPlaces(dto.Price, dto.AvailablePlaces);

        var country = await _unitOfWork.Countries.GetByIdAsync(dto.CountryId, cancellationToken)
            ?? throw new NotFoundException("Країна", dto.CountryId);

        _ = country;

        var tour = EntityMapper.ToEntity(dto);
        await _unitOfWork.Tours.AddAsync(tour, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _unitOfWork.Tours.GetByIdAsync(tour.Id, cancellationToken)
            ?? throw new NotFoundException("Тур", tour.Id);

        return EntityMapper.ToDto(created);
    }

    public async Task<TourDto> UpdateAsync(int id, UpdateTourDto dto, UserRole callerRole, CancellationToken cancellationToken = default)
    {
        EnsureCanManageTours(callerRole);
        ValidateTourDates(dto.StartDate, dto.EndDate);
        ValidatePriceAndPlaces(dto.Price, dto.AvailablePlaces);

        var tour = await _unitOfWork.Tours.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Тур", id);

        EntityMapper.ApplyUpdate(tour, dto);
        _unitOfWork.Tours.Update(tour);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return EntityMapper.ToDto(tour);
    }

    public async Task DeleteAsync(int id, UserRole callerRole, CancellationToken cancellationToken = default)
    {
        EnsureCanManageTours(callerRole);

        var tour = await _unitOfWork.Tours.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Тур", id);

        tour.IsActive = false;
        _unitOfWork.Tours.Update(tour);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static void EnsureCanManageTours(UserRole role)
    {
        if (role is not (UserRole.Manager or UserRole.Administrator))
        {
            throw new UnauthorizedBusinessException("Недостатньо прав для керування турами.");
        }
    }

    private static void ValidateTourDates(DateTime startDate, DateTime endDate)
    {
        if (endDate <= startDate)
        {
            throw new ValidationException("Дата завершення туру повинна бути пізніше дати початку.");
        }
    }

    private static void ValidatePriceAndPlaces(decimal price, int places)
    {
        if (price <= 0)
        {
            throw new ValidationException("Ціна туру повинна бути додатною.");
        }

        if (places <= 0)
        {
            throw new ValidationException("Кількість місць повинна бути додатною.");
        }
    }
}
