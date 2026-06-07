using CourseWork.Data.Enums;

namespace CourseWork.Data.Entities;

public class Tour
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TourType Type { get; set; }
    public int CountryId { get; set; }
    public int? RegionId { get; set; }
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsHot { get; set; }
    public int AvailablePlaces { get; set; }
    public bool IsActive { get; set; } = true;

    public Country Country { get; set; } = null!;
    public Region? Region { get; set; }
    public ICollection<TourBooking> Bookings { get; set; } = new List<TourBooking>();
}
