using CourseWork.Business.DTOs;
using CourseWork.Business.Mapping;
using CourseWork.Data.UnitOfWork;

namespace CourseWork.Business.Services;

public class ReferenceDataService : IReferenceDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReferenceDataService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<CountryDto>> GetCountriesAsync(CancellationToken cancellationToken = default)
    {
        var countries = await _unitOfWork.Countries.GetAllWithRegionsAsync(cancellationToken);
        return countries.Select(EntityMapper.ToDto).ToList();
    }

    public async Task<IReadOnlyList<TransportDto>> GetTransportsAsync(CancellationToken cancellationToken = default)
    {
        var transports = await _unitOfWork.Transports.GetAllAsync(cancellationToken);
        return transports.Select(EntityMapper.ToDto).ToList();
    }

    public async Task<IReadOnlyList<HotelRoomDto>> GetAvailableHotelRoomsAsync(CancellationToken cancellationToken = default)
    {
        var rooms = await _unitOfWork.HotelRooms.GetAvailableAsync(cancellationToken);
        return rooms.Select(EntityMapper.ToDto).ToList();
    }
}
