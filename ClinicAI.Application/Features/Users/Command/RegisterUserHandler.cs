using ClinicAI.Application.common.Models;
using ClinicAI.Application.Interfaces;
using ClinicAI.Domain.Entities;
using MediatR;

namespace ClinicAI.Application.Features.Users.Command
{
    public class RegisterUserHandler:IRequestHandler<RegisterUserCommand,Result<Guid>>
    {
        private readonly IClinicDbContext _context;
        public RegisterUserHandler(IClinicDbContext context)
        {
            _context= context??throw new ArgumentNullException(nameof(context));
        }
        public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var exists = _context.Users.Any(u => u.Email == request.Email);

            if (exists)
                return Result<Guid>.Failure("Email already exists");

            var user = new User
            {
                id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(user.id, "User registered successfully");
        }
    }
}
