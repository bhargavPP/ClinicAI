using ClinicAI.Domain.Entities;

namespace ClinicAI.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
