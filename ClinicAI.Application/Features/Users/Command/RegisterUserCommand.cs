using ClinicAI.Application.common.Models;
using ClinicAI.Application.DTOs;
using MediatR;

namespace ClinicAI.Application.Features.Users.Command
{
    public class RegisterUserCommand : IRequest<Result<AuthResponse>>
    {
        public String Email { get; set; }
        public String Password { get; set; }
        public String FullName { get; set; }
        public String Phone { get; set; }

        public String ConfirmPassword { get; set; }

    }
}
