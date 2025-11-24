using Domain.Entities;

namespace Domain.Interfaces;

public interface IGymSearchService
{
    Task<List<GymPlace>> SearchGymsAroundAddressAsync(string address);
}