using Microsoft.EntityFrameworkCore;
using Outclass.BuildingBlocks.Domain;
using Outclass.BuildingBlocks.Infrastructure.Persistence;
using Outclass.Document.Domain.Entities;

namespace Outclass.Document.Infrastructure.Persistence;

public class DocumentDbContext : BaseDbContext
{
    public DbSet<DynamicDocument> Documents => Set<DynamicDocument>();

    public DocumentDbContext(DbContextOptions<DocumentDbContext> options, ITenantContext tenantContext)
        : base(options, tenantContext) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DynamicDocument>(entity =>
        {
            entity.ToTable("documents");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntitySlug).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Data).HasColumnType("jsonb").IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.HasIndex(e => new { e.TenantId, e.EntitySlug });
            entity.HasIndex(e => e.TenantId);
        });
    }
}
