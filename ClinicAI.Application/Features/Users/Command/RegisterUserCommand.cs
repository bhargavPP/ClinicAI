using ClinicAI.Application.common.Models;
using MediatR;

namespace ClinicAI.Application.Features.Users.Command
{
    public class RegisterUserCommand:IRequest<Result<Guid>>
    {
        public String Email { get; set; }
        public String Password { get; set; }
        public String FullName { get; set; }

    }
}
