using ClinicAI.Application.common.Models;
using ClinicAI.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace ClinicAI.Application.Features.Users.Command
{
    public class LogoutCommandHandler
         : IRequestHandler<LogoutCommand, Result<bool>>
    {
        private readonly IClinicDbContext _context;

        public LogoutCommandHandler(IClinicDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(
            LogoutCommand request,
            CancellationToken cancellationToken)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken);

            if (token == null)
                return Result<bool>.Failure("Token not found");

            if (token.IsRevoked)
                return Result<bool>.Failure("Token already revoked");

            // ✅ Revoke token
            token.IsRevoked = true;
            token.RevokedReason = "User logged out";

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, "Logged out successfully");
        }
    }
}
