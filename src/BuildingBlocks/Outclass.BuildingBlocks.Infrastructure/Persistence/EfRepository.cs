using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Outclass.BuildingBlocks.Domain;

namespace Outclass.BuildingBlocks.Infrastructure.Persistence;

public class EfRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly BaseDbContext Context;
    protected readonly DbSet<T> DbSet;

    public EfRepository(BaseDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet.FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
    {
        return await DbSet.ToListAsync(ct);
    }

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        return await DbSet.Where(predicate).ToListAsync(ct);
    }

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await DbSet.AddAsync(entity, ct);
        await Context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        Context.Entry(entity).State = EntityState.Modified;
        await Context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        entity.MarkAsDeleted();
        await Context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(e => e.Id == id, ct);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
    {
        return predicate == null
            ? await DbSet.CountAsync(ct)
            : await DbSet.CountAsync(predicate, ct);
    }
}
