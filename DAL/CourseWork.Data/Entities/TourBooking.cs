using CourseWork.Data.Enums;

namespace CourseWork.Data.Entities;

public class TourBooking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TourId { get; set; }
    public DateTime BookingDate { get; set; }
    public int PlacesCount { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    public User User { get; set; } = null!;
    public Tour Tour { get; set; } = null!;
}
