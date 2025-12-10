using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Api.DTO.Gyms;
using Domain.Entities;
using Domain.Interfaces;
using Tools.DTOs;


namespace Tools.Services;

public class OsmGymPlaceSearchService : IGymSearchService
{
    private readonly HttpClient _httpClient;

    private readonly Dictionary<string, string> _sportTranslation = new()
    {
        { "basketball", "バスケ" },
        { "soccer", "サッカー" },
        { "badminton", "バドミントン" },
        { "tennis", "テニス" },
        { "swimming", "水泳" },
        { "fitness", "フィットネス" },
        { "yoga", "ヨーガ" },
        { "multi", "マルチ" },
        { "pool", "プール" },
        { "gim", "ギム" },
        { "dry", "ドライ" },
        { "dance", "ダンス" },
        { "dancing", "ダンシング" },
        { "exercise", "エクササイズ" },
        { "workout", "運動" },
        { "bicycle", "自転車" },
        { "cycling", "サイクリング" },
        { "kickboxing", "キックボクシング" }
    };

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
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "ja");
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ITSS_App/1.0"); // Bắt buộc

        var geoResponse = await _httpClient.GetFromJsonAsync<JsonArray>(geoUrl);
        if (geoResponse == null || geoResponse.Count == 0) return new List<GymPlace>();

        var searchLat = geoResponse[0]["lat"].ToString();
        var searchLon = geoResponse[0]["lon"].ToString();
        double originLat = double.Parse(searchLat);
        double originLon = double.Parse(searchLon);
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
            var resultList = new List<GymPlace>();

            foreach (var element in overpassData.Elements)
            {
                if (element.Tags == null) continue;

                // 1. Xử lý Tên (Ưu tiên tiếng Nhật như bài trước, hoặc tiếng Việt tùy bạn)
                string name = "Phòng tập không tên";
                if (element.Tags.TryGetValue("name:vi", out var viName)) name = viName;
                else if (element.Tags.TryGetValue("name", out var originName)) name = originName;

                // 2. Xử lý Môn thể thao (Tags "sport" trong OSM)
                var sportTags = new List<string>();
                if (element.Tags.TryGetValue("sport", out var sportRaw))
                {
                    // OSM trả về dạng "basketball;badminton"
                    var sports = sportRaw.Split(';');
                    foreach (var s in sports)
                    {
                        // Dịch sang tiếng Việt nếu có trong từ điển, không thì lấy nguyên gốc
                        var translated = _sportTranslation.TryGetValue(s, out var vn) ? vn : s;
                        sportTags.Add(translated);
                    }
                }

                // Nếu không có tag sport thì check tag leisure
                if (sportTags.Count == 0 && element.Tags.TryGetValue("leisure", out var leisure))
                {
                    if (leisure == "fitness_centre") sportTags.Add("フィットネス");
                }

                // 3. Xử lý Ảnh (OSM không lưu ảnh, ta phải tự random hoặc check tag 'image')
                string imgUrl = "https://images.pexels.com/photos/8933584/pexels-photo-8933584.jpeg";
                if (sportTags.Contains("Bơi lội"))
                    imgUrl = "https://images.pexels.com/photos/1415810/pexels-photo-1415810.jpeg";
                else if (sportTags.Contains("Bóng rổ"))
                    imgUrl = "https://images.pexels.com/photos/945471/pexels-photo-945471.jpeg";

                // 4. Xử lý Tọa độ & Địa chỉ
                double lat = element.Lat ?? element.Center?.Lat ?? 0;
                double lon = element.Lon ?? element.Center?.Lon ?? 0;
                string addr = GetAddressFromTags(element.Tags); // Hàm cũ của bạn
                resultList.Add(new GymPlace
                {
                    Id = Guid.NewGuid(),
                    // Lưu lại OsmId gốc để tham chiếu nếu cần
                    OsmId = $"{element.Type}/{element.Id}",
                    Name = name,
                    Address = GetAddressFromTags(element.Tags),
                    Latitude = element.Lat ?? element.Center?.Lat ?? 0,
                    Longitude = element.Lon ?? element.Center?.Lon ?? 0,
                    DistanceInMeters = CalculateDistance(originLat, originLon, lat, lon),
                    // Điền dữ liệu mới vào các trường vừa thêm
                    Sports = sportTags,
                    ImageUrl = imgUrl
                });
            }

            return resultList;
        }
        catch (Exception ex)
        {
            // Logging error here
            Console.WriteLine($"[ERROR] Error while searching for gym center {ex.Message}");
            return new List<GymPlace>();
        }
    }


    private string GetAddressFromTags(Dictionary<string, string> tags)
    {
        var parts = new List<string>();

        // Helper function nhỏ để lấy value ưu tiên tiếng Nhật
        string GetTagValue(string key)
        {
            if (tags.TryGetValue($"{key}:ja", out var valJa)) return valJa;
            if (tags.TryGetValue(key, out var val)) return val;
            return null;
        }

        var num = GetTagValue("addr:housenumber");
        var street = GetTagValue("addr:street");
        var city = GetTagValue("addr:city");

        if (num != null) parts.Add(num);
        if (street != null) parts.Add(street);
        if (city != null) parts.Add(city);

        return parts.Count > 0 ? string.Join(", ", parts) : "住所情報なし";
    }

    // 1. Thêm hàm tiện ích tính khoảng cách
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371e3; // Bán kính trái đất (mét)
        var phi1 = lat1 * Math.PI / 180;
        var phi2 = lat2 * Math.PI / 180;
        var deltaPhi = (lat2 - lat1) * Math.PI / 180;
        var deltaLambda = (lon2 - lon1) * Math.PI / 180;

        var a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2) +
                Math.Cos(phi1) * Math.Cos(phi2) *
                Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c; // Trả về mét
    }

    // 1. Thêm hàm implement mới
    public async Task<GymPlace?> GetGymPlaceByOsmIdAsync(string osmId)
    {
        // osmId format mong đợi: "node/12345" hoặc "way/67890"
        var parts = osmId.Split('/');
        if (parts.Length != 2) return null;

        string type = parts[0]; // node hoặc way
        string id = parts[1];

        // Query lấy đúng 1 phần tử theo ID
        string overpassQuery = $"""
                                [out:json][timeout:25];
                                {type}({id});
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
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var overpassData = JsonSerializer.Deserialize<OverpassResponse>(jsonString, options);

            if (overpassData?.Elements == null || !overpassData.Elements.Any())
            {
                return null;
            }

            // Lấy phần tử đầu tiên
            var element = overpassData.Elements.First();

            // Gọi hàm map dữ liệu (xem phần dưới)
            // Lưu ý: Detail thì DistanceInMeters = 0 hoặc null vì không có điểm gốc so sánh
            return MapToGymPlace(element, 0, 0);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Error getting gym detail: {ex.Message}");
            return null;
        }
    }

    // 2. Refactor logic Map (Trích xuất từ hàm SearchGymsAroundAddressAsync cũ)
    private GymPlace MapToGymPlace(Tools.DTOs.OverpassElement element, double originLat, double originLon)
    {
        if (element.Tags == null) return new GymPlace(); // Hoặc xử lý null tùy ý

// 1. Xử lý Tên (Ưu tiên tiếng Nhật nếu muốn giống ảnh)
        string name = "Unknown Place";
        var lat = element.Lat ?? element.Center?.Lat ?? 0;
        var lon = element.Lon ?? element.Center?.Lon ?? 0;
        // Thử lấy tên tiếng Nhật trước
        if (element.Tags.TryGetValue("name:ja", out var jaName)) name = jaName;
        else if (element.Tags.TryGetValue("name:vi", out var viName)) name = viName;
        else if (element.Tags.TryGetValue("name", out var defName)) name = defName;

        // 2. Xử lý Mô tả (Description) - Quan trọng để giống ảnh
        string? description = null;
        if (element.Tags.TryGetValue("description:ja", out var descJa)) description = descJa; // Lấy mô tả tiếng Nhật
        else if (element.Tags.TryGetValue("description", out var desc)) description = desc; // Lấy mô tả chung

        // Nếu không có mô tả, tạo description giả lập từ các tag khác để UI không bị trống
        if (string.IsNullOrEmpty(description) && element.Tags.TryGetValue("sport", out var s))
        {
            // Dịch sang tiếng Việt nếu có trong từ điển, không thì lấy nguyên gốc
            var translated = _sportTranslation.TryGetValue(s, out var vn) ? vn : s;
        }

        // 3. Xử lý Giờ mở cửa (Opening Hours)
        string? openingHours = null;
        if (element.Tags.TryGetValue("opening_hours", out var hours))
        {
            // OSM trả về dạng "Mo-Su 06:00-21:00", bạn có thể để nguyên hoặc format lại
            openingHours = hours;
        }

        var sportTags = new List<string>();
        if (element.Tags.TryGetValue("sport", out var sportRaw))
        {
            // OSM trả về dạng "basketball;badminton"
            var sports = sportRaw.Split(';');
            foreach (var si in sports)
            {
                // Dịch sang tiếng Việt nếu có trong từ điển, không thì lấy nguyên gốc
                var translated = _sportTranslation.TryGetValue(si, out var vn) ? vn : si;
                sportTags.Add(translated);
            }
            var sportsString = string.Join(", ", sportTags);
    
            description = $"{name} は {sportsString} にぴったりの場所です。";
        }

        // Nếu không có tag sport thì check tag leisure
        if (sportTags.Count == 0 && element.Tags.TryGetValue("leisure", out var leisure))
        {
            if (leisure == "fitness_centre") sportTags.Add("フィットネス");
        }

        // 3. Xử lý Ảnh (OSM không lưu ảnh, ta phải tự random hoặc check tag 'image')
        string imgUrl = "https://images.pexels.com/photos/8933584/pexels-photo-8933584.jpeg";
        if (sportTags.Contains("Bơi lội"))
            imgUrl = "https://images.pexels.com/photos/1415810/pexels-photo-1415810.jpeg";
        else if (sportTags.Contains("Bóng rổ"))
            imgUrl = "https://images.pexels.com/photos/945471/pexels-photo-945471.jpeg";

        // 4. Map vào Object
        var result = new GymPlace
        {
            Id = Guid.NewGuid(),
            OsmId = $"{element.Type}/{element.Id}",
            Name = name,
            Address = GetAddressFromTags(element.Tags),
            Latitude = element.Lat ?? element.Center?.Lat ?? 0,
            Longitude = element.Lon ?? element.Center?.Lon ?? 0,

            // Map các trường mới
            Description = description,
            OpeningHours = openingHours,

            // Giữ nguyên các trường cũ
            Sports = sportTags,
            ImageUrl = imgUrl,
            DistanceInMeters = CalculateDistance(originLat, originLon, lat, lon)
        };

        return result;
    }
}