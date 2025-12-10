using Domain.Entities;

namespace Domain.Interfaces;

public interface IGymSearchService
{
    Task<List<GymPlace>> SearchGymsAroundAddressAsync(string address);
    
    // Thêm hàm này
    Task<GymPlace?> GetGymPlaceByOsmIdAsync(string osmId);
}