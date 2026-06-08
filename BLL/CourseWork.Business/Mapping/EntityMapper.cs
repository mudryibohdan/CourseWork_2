using CourseWork.Business.DTOs;
using CourseWork.Data.Entities;
using CourseWork.Data.Enums;

namespace CourseWork.Business.Mapping;

public static class EntityMapper
{
    public static TourDto ToDto(Tour tour) => new(
        tour.Id,
        tour.Title,
        tour.Description,
        tour.Type,
        tour.Country.Name,
        tour.Region?.Name,
        tour.Price,
        tour.StartDate,
        tour.EndDate,
        tour.IsHot,
        tour.AvailablePlaces);

    public static UserDto ToDto(User user) => new(
        user.Id,
        user.Email,
        user.FirstName,
        user.LastName,
        user.Role);

    public static TourBookingDto ToDto(TourBooking booking) => new(
        booking.Id,
        booking.TourId,
        booking.Tour.Title,
        booking.PlacesCount,
        booking.TotalPrice,
        booking.Status,
        booking.BookingDate);

    public static TicketBookingDto ToDto(TicketBooking booking) => new(
        booking.Id,
        booking.BookingType,
        booking.BookingType == TicketBookingType.Transport
            ? booking.Transport!.Name
            : $"{booking.HotelRoom!.Hotel.Name} — кімната {booking.HotelRoom.RoomNumber}",
        booking.Quantity,
        booking.TotalPrice,
        booking.Status,
        booking.BookingDate);

    public static CountryDto ToDto(Country country) => new(
        country.Id,
        country.Name,
        country.Regions.Select(r => new RegionDto(r.Id, r.Name, r.CountryId)).ToList());

    public static TransportDto ToDto(Transport transport) => new(
        transport.Id,
        transport.Name,
        transport.Type,
        transport.Route,
        transport.Price,
        transport.DepartureTime,
        transport.AvailableSeats);

    public static HotelRoomDto ToDto(HotelRoom room) => new(
        room.Id,
        room.Hotel.Name,
        room.RoomNumber,
        room.PricePerNight,
        room.Capacity,
        room.IsAvailable);

    public static Tour ToEntity(CreateTourDto dto) => new()
    {
        Title = dto.Title,
        Description = dto.Description,
        Type = dto.Type,
        CountryId = dto.CountryId,
        RegionId = dto.RegionId,
        Price = dto.Price,
        StartDate = dto.StartDate,
        EndDate = dto.EndDate,
        IsHot = dto.IsHot,
        AvailablePlaces = dto.AvailablePlaces,
        IsActive = true
    };

    public static void ApplyUpdate(Tour tour, UpdateTourDto dto)
    {
        tour.Title = dto.Title;
        tour.Description = dto.Description;
        tour.Type = dto.Type;
        tour.CountryId = dto.CountryId;
        tour.RegionId = dto.RegionId;
        tour.Price = dto.Price;
        tour.StartDate = dto.StartDate;
        tour.EndDate = dto.EndDate;
        tour.IsHot = dto.IsHot;
        tour.AvailablePlaces = dto.AvailablePlaces;
        tour.IsActive = dto.IsActive;
    }
}
