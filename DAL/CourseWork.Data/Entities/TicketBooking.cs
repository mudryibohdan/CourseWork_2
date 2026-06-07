using CourseWork.Data.Enums;

namespace CourseWork.Data.Entities;

public class TicketBooking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public TicketBookingType BookingType { get; set; }
    public int? TransportId { get; set; }
    public int? HotelRoomId { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    public User User { get; set; } = null!;
    public Transport? Transport { get; set; }
    public HotelRoom? HotelRoom { get; set; }
}
