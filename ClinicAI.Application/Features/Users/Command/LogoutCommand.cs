using ClinicAI.Application.common.Models;
using MediatR;

namespace ClinicAI.Application.Features.Users.Command
{
    public record LogoutCommand(string RefreshToken)
         : IRequest<Result<bool>>;
}
