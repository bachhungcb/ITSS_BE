namespace Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IDeveloperRepository Developers { get; set; }
    IProjectRepository Projects { get; set; }
    int Complete();
}