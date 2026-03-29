using ClinicAI.Application.common.Models;
using ClinicAI.Application.DTOs;
using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Users.Command
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
    {
        private readonly IClinicDbContext _context;
        private readonly IJwtTokenService _jwt;

        public RefreshTokenCommandHandler(
               IClinicDbContext context,
               IJwtTokenService jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        public async Task<Result<AuthResponse>> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            // ✅ FIX: Include works only with EF namespace
            var storedToken = await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(
                    x => x.Token == request.RefreshToken,
                    cancellationToken);

            if (storedToken == null || !storedToken.IsActive)
                return Result<AuthResponse>.Failure("Invalid or expired refresh token");

            var user = storedToken.User;

            // ✅ Revoke old token
            storedToken.IsRevoked = true;
            storedToken.RevokedReason = "Replaced";

            // ✅ Generate new tokens
            var accessToken = _jwt.GenerateAccessToken(user);
            var refreshToken = _jwt.GenerateRefreshToken();

            // ✅ Save new refresh token
            _context.RefreshTokens.Add(new Domain.Entities.RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });

            await _context.SaveChangesAsync(cancellationToken);

            return Result<AuthResponse>.Success(
                new AuthResponse(
                    accessToken,
                    refreshToken,
                    3600,
                    new UserDto(user.Id, user.FullName, user.Email, user.Role)
                )
            );
        }
    }
}