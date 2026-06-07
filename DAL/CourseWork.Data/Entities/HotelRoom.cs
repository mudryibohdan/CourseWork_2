namespace CourseWork.Data.Entities;

public class HotelRoom
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public bool IsAvailable { get; set; } = true;

    public Hotel Hotel { get; set; } = null!;
    public ICollection<TicketBooking> Bookings { get; set; } = new List<TicketBooking>();
}
