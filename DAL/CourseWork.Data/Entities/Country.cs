namespace CourseWork.Data.Entities;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Region> Regions { get; set; } = new List<Region>();
    public ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
