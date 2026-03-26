using ClinicAI.Application.common.Models;
using MediatR;

namespace ClinicAI.Application.Features.Users.Command
{
    public record LoginUserCommand(
    string Email,
    string Password
) : IRequest<Result<string>>;
}
