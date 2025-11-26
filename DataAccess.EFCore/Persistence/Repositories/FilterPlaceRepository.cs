using Application.Interfaces;
using DataAccess.EFCore.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.EFCore.Persistence.Repositories;

public class FilterPlaceRepository : GenericRepository<FilterPlace>, IFilterPlaceRepository
{
    public FilterPlaceRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    // Hàm này phải trùng tên và kiểu trả về với Interface ở Bước 1
    public async Task<IEnumerable<FilterPlace>> GetByUserId(Guid userId)
    {
        return await _dbContext.Set<FilterPlace>()
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }
    
}