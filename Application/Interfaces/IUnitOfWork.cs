namespace Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
   IUserRepository UserRepository { get; }

   Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<int> Complete();
}