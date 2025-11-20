using Domain.Entities;

namespace Domain.Interfaces;

public interface IDeveloperRepository : IGenericRepository<Developer>
{
    IEnumerable<Developer> GetPopularDevelopers(int count);
}