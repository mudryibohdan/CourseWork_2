using CourseWork.Business.DTOs;
using CourseWork.Business.Exceptions;
using CourseWork.Business.Mapping;
using CourseWork.Data.Entities;
using CourseWork.Data.Enums;
using CourseWork.Data.UnitOfWork;

namespace CourseWork.Business.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TourBookingDto> BookTourAsync(
        int userId,
        CreateTourBookingDto dto,
        UserRole callerRole,
        CancellationToken cancellationToken = default)
    {
        EnsureCanBook(callerRole);

        if (dto.PlacesCount <= 0)
        {
            throw new ValidationException("Кількість місць для бронювання повинна бути додатною.");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("Користувач", userId);

        var tour = await _unitOfWork.Tours.GetByIdAsync(dto.TourId, cancellationToken)
            ?? throw new NotFoundException("Тур", dto.TourId);

        if (!tour.IsActive)
        {
            throw new ValidationException("Обраний тур недоступний для бронювання.");
        }

        if (tour.AvailablePlaces < dto.PlacesCount)
        {
            throw new InsufficientAvailabilityException(
                $"Недостатньо вільних місць. Доступно: {tour.AvailablePlaces}.");
        }

        var booking = new TourBooking
        {
            UserId = user.Id,
            TourId = tour.Id,
            PlacesCount = dto.PlacesCount,
            TotalPrice = tour.Price * dto.PlacesCount,
            BookingDate = DateTime.UtcNow,
            Status = BookingStatus.Confirmed
        };

        tour.AvailablePlaces -= dto.PlacesCount;

        await _unitOfWork.TourBookings.AddAsync(booking, cancellationToken);
        _unitOfWork.Tours.Update(tour);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        booking.Tour = tour;
        return EntityMapper.ToDto(booking);
    }

    public async Task<IReadOnlyList<TourBookingDto>> GetUserBookingsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var bookings = await _unitOfWork.TourBookings.GetByUserIdAsync(userId, cancellationToken);
        return bookings.Select(EntityMapper.ToDto).ToList();
    }

    public async Task CancelBookingAsync(int bookingId, int userId, UserRole callerRole, CancellationToken cancellationToken = default)
    {
        var booking = await _unitOfWork.TourBookings.GetByIdAsync(bookingId, cancellationToken)
            ?? throw new NotFoundException("Бронювання", bookingId);

        if (booking.UserId != userId && callerRole is not UserRole.Administrator)
        {
            throw new UnauthorizedBusinessException("Неможливо скасувати чуже бронювання.");
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            throw new ValidationException("Бронювання вже скасовано.");
        }

        var tour = await _unitOfWork.Tours.GetByIdAsync(booking.TourId, cancellationToken)
            ?? throw new NotFoundException("Тур", booking.TourId);

        tour.AvailablePlaces += booking.PlacesCount;
        booking.Status = BookingStatus.Cancelled;

        _unitOfWork.Tours.Update(tour);
        _unitOfWork.TourBookings.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static void EnsureCanBook(UserRole role)
    {
        if (role is UserRole.Guest)
        {
            throw new UnauthorizedBusinessException("Для бронювання необхідна реєстрація.");
        }
    }
}
