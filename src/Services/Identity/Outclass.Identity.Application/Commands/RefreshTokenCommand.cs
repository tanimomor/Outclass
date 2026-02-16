using MediatR;
using FluentValidation;

namespace Outclass.Identity.Application.Commands;

public record RefreshTokenCommand : IRequest<LoginResult>
{
    public string RefreshToken { get; init; } = default!;
}

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
