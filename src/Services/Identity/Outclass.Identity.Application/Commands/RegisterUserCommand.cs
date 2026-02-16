using MediatR;
using FluentValidation;

namespace Outclass.Identity.Application.Commands;

public record RegisterUserCommand : IRequest<RegisterUserResult>
{
    public Guid TenantId { get; init; }
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
}

public record RegisterUserResult
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = default!;
}

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(128);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TenantId).NotEmpty();
    }
}
