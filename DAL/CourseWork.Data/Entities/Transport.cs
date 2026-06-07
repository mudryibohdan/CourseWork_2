using CourseWork.Data.Enums;

namespace CourseWork.Data.Entities;

public class Transport
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TransportType Type { get; set; }
    public string Route { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime DepartureTime { get; set; }
    public int AvailableSeats { get; set; }

    public ICollection<TicketBooking> Bookings { get; set; } = new List<TicketBooking>();
}
