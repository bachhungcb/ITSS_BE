using Domain.Interfaces;

namespace Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository UserRepository { get; }
    IFilterPlaceRepository FilterPlaceRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<int> Complete();
}