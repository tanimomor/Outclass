using MediatR;
using Outclass.BuildingBlocks.Domain;
using Outclass.Identity.Domain.Entities;

namespace Outclass.Identity.Application.Queries.Handlers;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IRepository<User> _userRepository;

    public GetUserByIdQueryHandler(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };
    }
}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, UsersListDto>
{
    private readonly IRepository<User> _userRepository;

    public GetUsersQueryHandler(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UsersListDto> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var users = await _userRepository.GetAllAsync(ct);
        var totalCount = users.Count;

        var items = users
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            })
            .ToList();

        return new UsersListDto
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
