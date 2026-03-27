using ClinicAI.Domain.Entities;

namespace ClinicAI.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}
