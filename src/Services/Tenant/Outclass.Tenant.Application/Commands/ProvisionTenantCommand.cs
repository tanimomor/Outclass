using MediatR;
using FluentValidation;
using Outclass.Tenant.Domain.Entities;

namespace Outclass.Tenant.Application.Commands;

public record ProvisionTenantCommand : IRequest<ProvisionTenantResult>
{
    public string Name { get; init; } = default!;
    public string Slug { get; init; } = default!;
    public string AdminEmail { get; init; } = default!;
    public string AdminPassword { get; init; } = default!;
    public string AdminFirstName { get; init; } = default!;
    public string AdminLastName { get; init; } = default!;
    public TenantPlan Plan { get; init; } = TenantPlan.Free;
}

public record ProvisionTenantResult
{
    public Guid TenantId { get; init; }
    public string Name { get; init; } = default!;
    public string Slug { get; init; } = default!;
}

public class ProvisionTenantCommandValidator : AbstractValidator<ProvisionTenantCommand>
{
    public ProvisionTenantCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(100)
            .Matches("^[a-z0-9-]+$").WithMessage("Slug must be lowercase alphanumeric with dashes");
        RuleFor(x => x.AdminEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.AdminPassword).NotEmpty().MinimumLength(8);
        RuleFor(x => x.AdminFirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.AdminLastName).NotEmpty().MaximumLength(100);
    }
}
