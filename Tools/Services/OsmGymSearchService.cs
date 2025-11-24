using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Domain.Entities;
using Domain.Interfaces;
using Tools.DTOs;

namespace Tools.Services;

public class OsmGymPlaceSearchService : IGymSearchService
{
    private readonly HttpClient _httpClient;

    public OsmGymPlaceSearchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<GymPlace>> SearchGymsAroundAddressAsync(string address)
    {
        // BƯỚC 1: Lấy tọa độ từ địa chỉ (Dùng Nominatim)
        // Lưu ý: UrlEncode address
        var geoUrl =
            $"https://nominatim.openstreetmap.org/search?q={System.Net.WebUtility.UrlEncode(address)}&format=json&limit=1";
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ITSS_App/1.0"); // Bắt buộc

        var geoResponse = await _httpClient.GetFromJsonAsync<JsonArray>(geoUrl);
        if (geoResponse == null || geoResponse.Count == 0) return new List<GymPlace>();

        var searchLat = geoResponse[0]["lat"].ToString();
        var searchLon = geoResponse[0]["lon"].ToString();

        // BƯỚC 2: Tìm Gym bán kính 1000m quanh tọa độ đó (Dùng Overpass API)
        // QUERY ĐÃ TỐI ƯU (Dùng out center)
        string overpassQuery = $"""
                                    [out:json][timeout:25];
                                    (
                                      node["leisure"="fitness_centre"](around:10000, {searchLat}, {searchLon});
                                      node["sport"="fitness"](around:10000, {searchLat}, {searchLon});
                                      way["leisure"="fitness_centre"](around:10000, {searchLat}, {searchLon});
                                      way["sport"="fitness"](around:10000, {searchLat}, {searchLon});
                                    );
                                    out center; 
                                """;

        var overpassUrl = "https://overpass-api.de/api/interpreter";
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("data", overpassQuery)
        });

        try
        {
            var response = await _httpClient.PostAsync(overpassUrl, content);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();

            // 1. DESERIALIZE JSON về DTO
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var overpassData = JsonSerializer.Deserialize<OverpassResponse>(jsonString, options);

            if (overpassData?.Elements == null || !overpassData.Elements.Any())
            {
                return new List<GymPlace>();
            }

            // 2. MAP DTO -> DOMAIN ENTITY
            var gyms = new List<GymPlace>();

            foreach (var element in overpassData.Elements)
            {
                // Bỏ qua nếu không có tags (không có tên hoặc thông tin)
                if (element.Tags == null) continue;

                // Xử lý tọa độ: Nếu là Node lấy Lat/Lon, nếu là Way lấy Center.Lat/Center.Lon
                double? latitude = element.Lat ?? element.Center?.Lat;
                double? longitude = element.Lon ?? element.Center?.Lon;

                // Nếu không lấy được tọa độ thì bỏ qua
                if (!latitude.HasValue || !longitude.HasValue) continue;

                // Lấy tên, nếu không có tên thì lấy tạm loại hình
                string name = element.Tags.ContainsKey("name")
                    ? element.Tags["name"]
                    : "Unknown Gym";

                // Lấy địa chỉ nếu có (OSM dùng các tag addr:housenumber, addr:street...)
                string addressDetail = GetAddressFromTags(element.Tags);

                gyms.Add(new GymPlace
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Address = addressDetail,
                    Latitude = latitude.Value,
                    Longitude = longitude.Value,
                    OsmId = $"{element.Type}/{element.Id}" // Lưu lại để debug: node/12345
                });
            }

            return gyms;
        }
        catch (Exception ex)
        {
            // Logging error here
            Console.WriteLine($"[ERROR] Error while searching for gym center {ex.Message}");
            return new List<GymPlace>();
        }
    }

    // DTO nội bộ để hứng JSON từ OSM
    private class OsmResultDto
    {
        public long osm_id { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string display_name { get; set; }
        public string type { get; set; }
    }

    private string GetAddressFromTags(Dictionary<string, string> tags)
    {
        var parts = new List<string>();
        if (tags.TryGetValue("addr:housenumber", out var num)) parts.Add(num);
        if (tags.TryGetValue("addr:street", out var street)) parts.Add(street);
        if (tags.TryGetValue("addr:city", out var city)) parts.Add(city);

        return parts.Count > 0 ? string.Join(", ", parts) : "No address info";
    }
}