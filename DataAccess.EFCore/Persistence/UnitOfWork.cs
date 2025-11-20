using Application.Interfaces;
using DataAccess.EFCore.Context;
using DataAccess.EFCore.Persistence.Repositories;

namespace DataAccess.EFCore.Persistence;

public class UnitOfWork : IUnitOfWork
{
    // 1. DbContext
    private readonly ApplicationDbContext _context;

    // 2. Repository

    #region Repositories

    public IUserRepository UserRepository { get; }

    #endregion

    // 3. Constructor
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;

        // Khởi tạo tất cả các repository của bạn ở đây
        // Bằng cách này, tất cả repository đều dùng CHUNG một DbContext
        UserRepository = new UserRepository(_context);
        // Nếu có repository khác (ví dụ: IPostRepository), 
        // bạn cũng sẽ khởi tạo nó ở đây.
    }

    // 4. Triển khai SaveChangesAsync
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    // 5. Triển khai Complete (phiên bản đồng bộ)
    public Task<int> Complete()
    {
        return _context.SaveChanges();
    }

    // 6. Triển khai Dispose (từ IDisposable)
    public void Dispose()
    {
        _context.Dispose();
    }
}