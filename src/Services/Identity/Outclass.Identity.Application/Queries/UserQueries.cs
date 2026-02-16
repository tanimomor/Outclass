using MediatR;

namespace Outclass.Identity.Application.Queries;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto?>;

public record GetUsersQuery(int Page = 1, int PageSize = 20) : IRequest<UsersListDto>;

public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = new List<string>();
}

public record UsersListDto
{
    public IReadOnlyList<UserDto> Items { get; init; } = new List<UserDto>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}
