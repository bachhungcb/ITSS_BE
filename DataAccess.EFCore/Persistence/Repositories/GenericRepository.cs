using System.Linq.Expressions;
using Application.Interfaces;
using DataAccess.EFCore.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.EFCore.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext _dbContext;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T?> GetById(Guid Id)
    {
        return await _dbContext.Set<T>().FindAsync(Id);
    }

    public IQueryable<T> GetAllAsQueryable()
    {
        // Trả về DbSet (cho phép lọc tiếp trên CSDL).
        // Thêm AsNoTracking() để tối ưu cho các hoạt động CHỈ ĐỌC,
        // nó bảo EF Core không cần theo dõi các đối tượng này.
        return _dbContext.Set<T>().AsNoTracking();
    }
    

    public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
    {
        // Where() trả về IQueryable, cho phép lọc trên CSDL
        // AsNoTracking() cũng là một tối ưu tốt ở đây.
        return Queryable.Where(_dbContext.Set<T>(), expression).AsNoTracking();
    }

    public void Add(T entity)
    {
        // Chỉ đánh dấu trạng thái (State) là 'Added'.
        // CHƯA gọi CSDL.
        _dbContext.Set<T>().Add(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().AddRange(entities);
    }

    public void Remove(T entity)
    {
        // Chỉ đánh dấu trạng thái (State) là 'Deleted'.
        // CHƯA gọi CSDL.
        _dbContext.Set<T>().Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().RemoveRange(entities);
    }
    
    public void Update(T entity)
    {
        _dbContext.Set<T>().Update(entity);
    }
}