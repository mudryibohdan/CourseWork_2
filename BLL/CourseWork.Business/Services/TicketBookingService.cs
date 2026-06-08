using CourseWork.Business.DTOs;
using CourseWork.Business.Exceptions;
using CourseWork.Business.Mapping;
using CourseWork.Data.Entities;
using CourseWork.Data.Enums;
using CourseWork.Data.UnitOfWork;

namespace CourseWork.Business.Services;

public class TicketBookingService : ITicketBookingService
{
    private const int MinimumNights = 1;

    private readonly IUnitOfWork _unitOfWork;

    public TicketBookingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketBookingDto> BookTransportAsync(
        int userId,
        CreateTransportTicketBookingDto dto,
        UserRole callerRole,
        CancellationToken cancellationToken = default)
    {
        EnsureCanBook(callerRole);

        if (dto.Quantity <= 0)
        {
            throw new ValidationException("Кількість квитків повинна бути додатною.");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("Користувач", userId);

        var transport = await _unitOfWork.Transports.GetByIdAsync(dto.TransportId, cancellationToken)
            ?? throw new NotFoundException("Транспорт", dto.TransportId);

        if (transport.AvailableSeats < dto.Quantity)
        {
            throw new InsufficientAvailabilityException(
                $"Недостатньо вільних місць. Доступно: {transport.AvailableSeats}.");
        }

        var booking = new TicketBooking
        {
            UserId = user.Id,
            BookingType = TicketBookingType.Transport,
            TransportId = transport.Id,
            Quantity = dto.Quantity,
            TotalPrice = transport.Price * dto.Quantity,
            BookingDate = DateTime.UtcNow,
            Status = BookingStatus.Confirmed
        };

        transport.AvailableSeats -= dto.Quantity;

        await _unitOfWork.TicketBookings.AddAsync(booking, cancellationToken);
        _unitOfWork.Transports.Update(transport);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        booking.Transport = transport;
        return EntityMapper.ToDto(booking);
    }

    public async Task<TicketBookingDto> BookHotelRoomAsync(
        int userId,
        CreateHotelRoomBookingDto dto,
        UserRole callerRole,
        CancellationToken cancellationToken = default)
    {
        EnsureCanBook(callerRole);

        if (dto.CheckOutDate <= dto.CheckInDate)
        {
            throw new ValidationException("Дата виїзду повинна бути пізніше дати заїзду.");
        }

        var nights = (dto.CheckOutDate.Date - dto.CheckInDate.Date).Days;
        if (nights < MinimumNights)
        {
            throw new ValidationException($"Мінімальна тривалість проживання — {MinimumNights} ніч.");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("Користувач", userId);

        var room = await _unitOfWork.HotelRooms.GetByIdAsync(dto.HotelRoomId, cancellationToken)
            ?? throw new NotFoundException("Номер готелю", dto.HotelRoomId);

        if (!room.IsAvailable)
        {
            throw new InsufficientAvailabilityException("Обраний номер недоступний.");
        }

        var booking = new TicketBooking
        {
            UserId = user.Id,
            BookingType = TicketBookingType.HotelRoom,
            HotelRoomId = room.Id,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            Quantity = nights,
            TotalPrice = room.PricePerNight * nights,
            BookingDate = DateTime.UtcNow,
            Status = BookingStatus.Confirmed
        };

        room.IsAvailable = false;

        await _unitOfWork.TicketBookings.AddAsync(booking, cancellationToken);
        _unitOfWork.HotelRooms.Update(room);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        booking.HotelRoom = room;
        booking.HotelRoom.Hotel = room.Hotel;
        return EntityMapper.ToDto(booking);
    }

    public async Task<IReadOnlyList<TicketBookingDto>> GetUserBookingsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var bookings = await _unitOfWork.TicketBookings.GetByUserIdAsync(userId, cancellationToken);
        return bookings.Select(EntityMapper.ToDto).ToList();
    }

    private static void EnsureCanBook(UserRole role)
    {
        if (role is UserRole.Guest)
        {
            throw new UnauthorizedBusinessException("Для бронювання необхідна реєстрація.");
        }
    }
}
