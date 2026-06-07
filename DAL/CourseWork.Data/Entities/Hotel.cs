namespace CourseWork.Data.Entities;

public class Hotel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int RegionId { get; set; }
    public int StarRating { get; set; }

    public Region Region { get; set; } = null!;
    public ICollection<HotelRoom> Rooms { get; set; } = new List<HotelRoom>();
}
