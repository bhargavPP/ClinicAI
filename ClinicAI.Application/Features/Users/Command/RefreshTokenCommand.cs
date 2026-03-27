using ClinicAI.Application.common.Models;
using ClinicAI.Application.DTOs;
using MediatR;

namespace ClinicAI.Application.Features.Users.Command
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponse>>
    {
    }
}
