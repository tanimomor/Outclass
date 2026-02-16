using Microsoft.EntityFrameworkCore;
using Outclass.BuildingBlocks.Domain;
using Outclass.BuildingBlocks.Infrastructure.Persistence;
using Outclass.FileStorage.Domain.Entities;

namespace Outclass.FileStorage.Infrastructure.Persistence;

public class FileStorageDbContext : BaseDbContext
{
    public DbSet<FileMetadata> Files => Set<FileMetadata>();

    public FileStorageDbContext(DbContextOptions<FileStorageDbContext> options, ITenantContext tenantContext)
        : base(options, tenantContext) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<FileMetadata>(e =>
        {
            e.ToTable("files");
            e.HasKey(x => x.Id);
            e.Property(x => x.FileName).HasMaxLength(500).IsRequired();
            e.Property(x => x.ContentType).HasMaxLength(200);
            e.Property(x => x.StorageKey).HasMaxLength(1024).IsRequired();
            e.HasIndex(x => new { x.TenantId, x.DocumentId });
        });
    }
}
