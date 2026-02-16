using Outclass.Identity.Domain.Entities;

namespace Outclass.Identity.Application.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user, IEnumerable<string> roles);
    string GenerateRefreshToken();
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
