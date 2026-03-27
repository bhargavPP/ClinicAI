using MediatR;
using ClinicAI.Application.DTOs;

namespace ClinicAI.Application.Features.Users.Queries
{
    public record LoginUserQuery(
        string Email,
        string Password
    ) : IRequest<AuthResponse>;
}
