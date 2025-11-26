using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class GymPlace : BaseEntity
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    
    // Position
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public string? OsmId { get; set; }
    
    [NotMapped] 
    public double? DistanceInMeters { get; set; }

    // --- THÊM CÁC TRƯỜNG MỚI ---
    [NotMapped] // Không lưu vào database, chỉ dùng để hứng dữ liệu từ API
    public List<string> Sports { get; set; } = new();
    
    [NotMapped]
    public string? ImageUrl { get; set; }
}