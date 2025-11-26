using System.Text.Json.Serialization;

namespace Api.DTO.Gyms;

public class GymResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    
    [JsonPropertyName("image_url")] 
    public string ImageUrl { get; set; }
    
    public SportInfo Sports { get; set; }
    public LocationInfo Location { get; set; }
}

public class SportInfo
{
    public List<string> Tags { get; set; } = new();
}

public class LocationInfo
{
    public string Address { get; set; }
    public Coordinates Coordinates { get; set; }
}

public class Coordinates
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}