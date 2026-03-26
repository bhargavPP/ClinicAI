using ClinicAI.Application.common.Models;
using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Users.Command
{
    public class LoginUserHandler:IRequestHandler<LoginUserCommand,Result<string>>
    {
        private readonly IClinicDbContext _context;
        private readonly IJwtTokenService _jwt; 
        public LoginUserHandler(IClinicDbContext context, IJwtTokenService jwt)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _jwt=jwt ?? throw new ArgumentNullException(nameof(jwt));
        }

        public async Task<Result<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
           var user = await _context.Users.FirstOrDefaultAsync(u=>u.Email == request.Email);

            if (user == null)
                return Result<string>.Failure("Invalid Credentials");

            var valid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if(!valid)
                return Result<string>.Failure("Invalid Credentials");

            var token = _jwt.GenerateToken(user);

            return Result<string>.Success(token,"Login Successful");
        }
    }
}
