using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User> FindByEmailAsync(string email);
    Task<User> FindByResetTokenAsync(string tokenHash);
}