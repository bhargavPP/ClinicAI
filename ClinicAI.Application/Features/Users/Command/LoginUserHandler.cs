using ClinicAI.Application.common.Models;
using ClinicAI.Application.DTOs;
using ClinicAI.Application.Interfaces;
using ClinicAI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicAI.Application.Features.Users.Command
{
    public class LoginUserHandler
        : IRequestHandler<LoginUserCommand, Result<AuthResponse>>
    {
        private readonly IClinicDbContext _context;
        private readonly IJwtTokenService _jwt;
        private readonly ILogger<LoginUserHandler> _logger;
        public LoginUserHandler(IClinicDbContext context, IJwtTokenService jwt,ILogger<LoginUserHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _jwt = jwt ?? throw new ArgumentNullException(nameof(jwt));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<AuthResponse>> Handle(
            LoginUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User not found: {Email}", request.Email);
                return Result<AuthResponse>.Failure("Invalid Credentials");
            }
            var valid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!valid)
            {
                _logger.LogWarning("Invalid Password: {Email}", request.Email);
                return Result<AuthResponse>.Failure("Invalid Credentials");
            }
            // ✅ Generate tokens
            var accessToken = _jwt.GenerateAccessToken(user);
            var refreshToken = _jwt.GenerateRefreshToken();

            // ✅ Save refresh token
            _context.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogWarning("token saved");
            // ✅ Return correct response
            return Result<AuthResponse>.Success(
                new AuthResponse(
                    accessToken,
                    refreshToken,
                    3600,
                    new UserDto(user.Id, user.FullName, user.Email, user.Role)
                ),
                "Login Successful"
            );
        }
    }
}
