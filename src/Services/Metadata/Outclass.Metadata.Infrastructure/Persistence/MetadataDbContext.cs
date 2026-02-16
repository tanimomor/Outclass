using Microsoft.EntityFrameworkCore;
using Outclass.BuildingBlocks.Domain;
using Outclass.BuildingBlocks.Infrastructure.Persistence;
using Outclass.Metadata.Domain.Entities;

namespace Outclass.Metadata.Infrastructure.Persistence;

public class MetadataDbContext : BaseDbContext
{
    public DbSet<EntityDefinition> EntityDefinitions => Set<EntityDefinition>();
    public DbSet<FieldDefinition> FieldDefinitions => Set<FieldDefinition>();

    public MetadataDbContext(DbContextOptions<MetadataDbContext> options, ITenantContext tenantContext)
        : base(options, tenantContext) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EntityDefinition>(entity =>
        {
            entity.ToTable("entity_definitions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Slug).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => new { e.TenantId, e.Slug }).IsUnique();
            entity.HasMany(e => e.Fields).WithOne().HasForeignKey(f => f.EntityDefinitionId);
        });

        modelBuilder.Entity<FieldDefinition>(entity =>
        {
            entity.ToTable("field_definitions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Slug).HasMaxLength(100).IsRequired();
            entity.Property(e => e.FieldType).HasConversion<string>().HasMaxLength(50);
            entity.HasIndex(e => new { e.EntityDefinitionId, e.Slug }).IsUnique();
        });
    }
}
