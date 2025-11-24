using System.Text.Json.Serialization;

namespace Tools.DTOs;

// Class bao bọc bên ngoài cùng
public class OverpassResponse
{
    [JsonPropertyName("elements")]
    public List<OverpassElement> Elements { get; set; } = new();
}

// Đại diện cho một địa điểm (Node hoặc Way)
public class OverpassElement
{
    [JsonPropertyName("type")]
    public string Type { get; set; } // "node" hoặc "way"

    [JsonPropertyName("id")]
    public long Id { get; set; }

    // Dành cho Node: Tọa độ nằm trực tiếp ở đây
    [JsonPropertyName("lat")]
    public double? Lat { get; set; }

    [JsonPropertyName("lon")]
    public double? Lon { get; set; }

    // Dành cho Way (Tòa nhà): Tọa độ nằm trong object center
    [JsonPropertyName("center")]
    public OverpassCenterInfo? Center { get; set; }

    // Các thông tin chi tiết: Tên, địa chỉ, loại hình...
    [JsonPropertyName("tags")]
    public Dictionary<string, string>? Tags { get; set; }
}

public class OverpassCenterInfo
{
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lon")]
    public double Lon { get; set; }
}