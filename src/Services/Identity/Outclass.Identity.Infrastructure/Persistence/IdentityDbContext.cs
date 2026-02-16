using Microsoft.EntityFrameworkCore;
using Outclass.BuildingBlocks.Domain;
using Outclass.BuildingBlocks.Infrastructure.Persistence;
using Outclass.Identity.Domain.Entities;

namespace Outclass.Identity.Infrastructure.Persistence;

public class IdentityDbContext : BaseDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, ITenantContext tenantContext)
        : base(options, tenantContext)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => new { e.TenantId, e.Email }).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.Property(e => e.RefreshToken).HasMaxLength(512);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => new { e.TenantId, e.Name }).IsUnique();
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("user_roles");
            entity.HasKey(e => new { e.UserId, e.RoleId });
            entity.HasOne(e => e.Role).WithMany().HasForeignKey(e => e.RoleId);
        });
    }
}
