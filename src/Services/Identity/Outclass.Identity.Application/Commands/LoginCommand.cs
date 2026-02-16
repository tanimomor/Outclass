using MediatR;
using FluentValidation;

namespace Outclass.Identity.Application.Commands;

public record LoginCommand : IRequest<LoginResult>
{
    public Guid TenantId { get; init; }
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
}

public record LoginResult
{
    public string AccessToken { get; init; } = default!;
    public string RefreshToken { get; init; } = default!;
    public DateTime ExpiresAt { get; init; }
    public Guid UserId { get; init; }
    public string Email { get; init; } = default!;
    public IReadOnlyList<string> Roles { get; init; } = new List<string>();
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.TenantId).NotEmpty();
    }
}
