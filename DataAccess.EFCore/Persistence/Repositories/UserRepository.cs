using Application.Interfaces;
using DataAccess.EFCore.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.EFCore.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<User> FindByEmailAsync(string email)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> FindByResetTokenAsync(string tokenHash)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.PasswordResetToken == tokenHash);
    }
    
}