using Microsoft.EntityFrameworkCore;
using Outclass.BuildingBlocks.Domain;
using Outclass.BuildingBlocks.Infrastructure.Persistence;
using Outclass.Tenant.Domain.Entities;

namespace Outclass.Tenant.Infrastructure.Persistence;

public class TenantDbContext : BaseDbContext
{
    public DbSet<TenantEntity> Tenants => Set<TenantEntity>();
    public DbSet<TenantSetting> TenantSettings => Set<TenantSetting>();

    public TenantDbContext(DbContextOptions<TenantDbContext> options, ITenantContext tenantContext)
        : base(options, tenantContext)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TenantEntity>(entity =>
        {
            entity.ToTable("tenants");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Slug).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.Plan).HasConversion<string>().HasMaxLength(50);
            entity.HasMany(e => e.Settings).WithOne().HasForeignKey(s => s.TenantEntityId);
        });

        modelBuilder.Entity<TenantSetting>(entity =>
        {
            entity.ToTable("tenant_settings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).HasMaxLength(256).IsRequired();
            entity.Property(e => e.Value).HasMaxLength(4096);
        });
    }
}
