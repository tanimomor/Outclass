using Microsoft.EntityFrameworkCore;
using Outclass.BuildingBlocks.Domain;
using Outclass.BuildingBlocks.Infrastructure.Persistence;
using Outclass.Workflow.Domain.Entities;

namespace Outclass.Workflow.Infrastructure.Persistence;

public class WorkflowDbContext : BaseDbContext
{
    public DbSet<WorkflowDefinition> WorkflowDefinitions => Set<WorkflowDefinition>();
    public DbSet<WorkflowTransitionRule> TransitionRules => Set<WorkflowTransitionRule>();
    public DbSet<WorkflowInstance> WorkflowInstances => Set<WorkflowInstance>();
    public DbSet<WorkflowTransitionLog> TransitionLogs => Set<WorkflowTransitionLog>();

    public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options, ITenantContext tenantContext)
        : base(options, tenantContext) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WorkflowDefinition>(e =>
        {
            e.ToTable("workflow_definitions");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.EntitySlug).HasMaxLength(100).IsRequired();
            e.Property(x => x.InitialState).HasMaxLength(50).IsRequired();
            e.HasIndex(x => new { x.TenantId, x.EntitySlug });
            e.HasMany(x => x.Transitions).WithOne().HasForeignKey(t => t.WorkflowDefinitionId);
        });

        modelBuilder.Entity<WorkflowTransitionRule>(e =>
        {
            e.ToTable("workflow_transition_rules");
            e.HasKey(x => x.Id);
            e.Property(x => x.FromState).HasMaxLength(50).IsRequired();
            e.Property(x => x.ToState).HasMaxLength(50).IsRequired();
            e.Property(x => x.RequiredRole).HasMaxLength(100);
        });

        modelBuilder.Entity<WorkflowInstance>(e =>
        {
            e.ToTable("workflow_instances");
            e.HasKey(x => x.Id);
            e.Property(x => x.CurrentState).HasMaxLength(50).IsRequired();
            e.HasIndex(x => new { x.TenantId, x.DocumentId });
            e.HasMany(x => x.TransitionLogs).WithOne().HasForeignKey(l => l.WorkflowInstanceId);
        });

        modelBuilder.Entity<WorkflowTransitionLog>(e =>
        {
            e.ToTable("workflow_transition_logs");
            e.HasKey(x => x.Id);
            e.Property(x => x.FromState).HasMaxLength(50);
            e.Property(x => x.ToState).HasMaxLength(50);
        });
    }
}
