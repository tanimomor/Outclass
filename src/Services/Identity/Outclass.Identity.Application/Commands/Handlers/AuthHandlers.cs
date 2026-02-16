using MediatR;
using Microsoft.Extensions.Logging;
using Outclass.BuildingBlocks.Domain;
using Outclass.Identity.Application.Services;
using Outclass.Identity.Domain.Entities;

namespace Outclass.Identity.Application.Commands.Handlers;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(
        IRepository<User> userRepository,
        IRepository<Role> roleRepository,
        IPasswordHasher passwordHasher,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var existingUsers = await _userRepository.FindAsync(u => u.Email == request.Email.ToLowerInvariant(), ct);
        if (existingUsers.Count > 0)
            throw new ConflictException($"User with email '{request.Email}' already exists.");

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(request.TenantId, request.Email, passwordHash, request.FirstName, request.LastName);

        // Assign default 'Member' role
        var roles = await _roleRepository.FindAsync(r => r.Name == "Member", ct);
        var memberRole = roles.FirstOrDefault();
        if (memberRole != null)
        {
            user.AssignRole(memberRole);
        }

        await _userRepository.AddAsync(user, ct);

        _logger.LogInformation("User {UserId} registered with email {Email} in tenant {TenantId}",
            user.Id, user.Email, user.TenantId);

        return new RegisterUserResult { UserId = user.Id, Email = user.Email };
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IRepository<User> _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IRepository<User> userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken ct)
    {
        var users = await _userRepository.FindAsync(u => u.Email == request.Email.ToLowerInvariant(), ct);
        var user = users.FirstOrDefault()
                   ?? throw new NotFoundException("User", Guid.Empty);

        if (!user.IsActive)
            throw new ForbiddenException("User account is deactivated.");

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new ForbiddenException("Invalid credentials.");

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddHours(1);

        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        user.RecordLogin();
        await _userRepository.UpdateAsync(user, ct);

        _logger.LogInformation("User {UserId} logged in", user.Id);

        return new LoginResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            Email = user.Email,
            Roles = roles
        };
    }
}
