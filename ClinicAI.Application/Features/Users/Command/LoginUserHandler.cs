using ClinicAI.Application.common.Models;
using ClinicAI.Application.DTOs;
using ClinicAI.Application.Interfaces;
using ClinicAI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Users.Command
{
    public class LoginUserHandler
        : IRequestHandler<LoginUserCommand, Result<AuthResponse>>
    {
        private readonly IClinicDbContext _context;
        private readonly IJwtTokenService _jwt;

        public LoginUserHandler(IClinicDbContext context, IJwtTokenService jwt)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _jwt = jwt ?? throw new ArgumentNullException(nameof(jwt));
        }

        public async Task<Result<AuthResponse>> Handle(
            LoginUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null)
                return Result<AuthResponse>.Failure("Invalid Credentials");

            var valid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!valid)
                return Result<AuthResponse>.Failure("Invalid Credentials");

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
