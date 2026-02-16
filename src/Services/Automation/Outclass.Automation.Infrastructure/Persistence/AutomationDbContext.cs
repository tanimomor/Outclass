using Microsoft.EntityFrameworkCore;
using Outclass.BuildingBlocks.Domain;
using Outclass.BuildingBlocks.Infrastructure.Persistence;
using Outclass.Automation.Domain.Entities;

namespace Outclass.Automation.Infrastructure.Persistence;

public class AutomationDbContext : BaseDbContext
{
    public DbSet<AutomationRule> AutomationRules => Set<AutomationRule>();
    public DbSet<AutomationExecutionLog> ExecutionLogs => Set<AutomationExecutionLog>();

    public AutomationDbContext(DbContextOptions<AutomationDbContext> options, ITenantContext tenantContext)
        : base(options, tenantContext) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AutomationRule>(e =>
        {
            e.ToTable("automation_rules");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.TriggerEvent).HasMaxLength(200).IsRequired();
            e.Property(x => x.ActionType).HasConversion<string>().HasMaxLength(50);
            e.Property(x => x.ActionPayload).HasColumnType("jsonb");
            e.HasIndex(x => new { x.TenantId, x.TriggerEvent });
        });

        modelBuilder.Entity<AutomationExecutionLog>(e =>
        {
            e.ToTable("automation_execution_logs");
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.TenantId, x.AutomationRuleId });
            e.HasIndex(x => x.ExecutedAt);
        });
    }
}
