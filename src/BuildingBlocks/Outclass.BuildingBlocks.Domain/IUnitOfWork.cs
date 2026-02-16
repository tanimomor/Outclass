namespace Outclass.BuildingBlocks.Domain;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
