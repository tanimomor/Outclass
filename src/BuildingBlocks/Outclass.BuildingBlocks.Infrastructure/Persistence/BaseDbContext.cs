using Microsoft.EntityFrameworkCore;
using Outclass.BuildingBlocks.Domain;

namespace Outclass.BuildingBlocks.Infrastructure.Persistence;

public abstract class BaseDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected BaseDbContext(DbContextOptions options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("outbox_messages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EventType).HasMaxLength(256).IsRequired();
            entity.Property(e => e.Payload).IsRequired();
            entity.HasIndex(e => new { e.ProcessedAt, e.CreatedAt });
        });

        ApplyTenantQueryFilters(modelBuilder);
    }

    private void ApplyTenantQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(BaseDbContext)
                    .GetMethod(nameof(ApplyTenantFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                    .MakeGenericMethod(entityType.ClrType);
                method.Invoke(this, new object[] { modelBuilder });
            }
        }
    }

    private void ApplyTenantFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : BaseEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.TenantId == _tenantContext.TenantId && !e.IsDeleted);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.TenantId == Guid.Empty && _tenantContext.IsResolved)
                        entry.Entity.SetTenant(_tenantContext.TenantId);
                    entry.Entity.SetCreatedBy("system");
                    break;
                case EntityState.Modified:
                    entry.Entity.SetAudit("system");
                    break;
            }
        }

        return await base.SaveChangesAsync(ct);
    }
}
