namespace Domain.Entities;

public class GymPlace : BaseEntity
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    
    //Position
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    // OPTIONAL: Save Open Street Map raw data for auditing
    public string? OsmId { get; set; }
}