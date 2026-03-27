using ClinicAI.Application.common.Models;
using ClinicAI.Application.DTOs;
using ClinicAI.Application.Interfaces;
using ClinicAI.Domain.Entities;
using MediatR;

namespace ClinicAI.Application.Features.Users.Command
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponse>>
    {
        private readonly IClinicDbContext _context;
        private readonly IJwtTokenService _jwtTokenService;
        public RegisterUserHandler(IClinicDbContext context,IJwtTokenService jwtTokenService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        }
        public async Task<Result<AuthResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (request.Password != request.ConfirmPassword)
                throw new Exception("Passwords do not match");

            var exists = _context.Users.Any(u => u.Email == request.Email);

            if (exists)
                throw new Exception("Email already exists");

            var user = new User
            {
                id = Guid.NewGuid(),
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            return Result<AuthResponse>.Success(
                  new AuthResponse(
                      accessToken,
                      refreshToken,
                      3600,
                      new UserDto(user.id, user.FullName, user.Email, user.Role)
                  ),
                  "User registered successfully"
              );
        }
    }
}
