using ClinicAI.Application.common.Models;
using ClinicAI.Application.DTOs;
using ClinicAI.Application.Interfaces;
using ClinicAI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Features.Users.Command
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponse>>
    {
        private readonly IClinicDbContext _context;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IBackgroundJobService _jobService;
        public RegisterUserHandler(IClinicDbContext context,IJwtTokenService jwtTokenService,IBackgroundJobService backgroundJobService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
            _jobService = backgroundJobService ?? throw new ArgumentNullException(nameof(backgroundJobService));
        }
        public async Task<Result<AuthResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (request.Password != request.ConfirmPassword)
                throw new Exception("Passwords do not match");

            var exists =await _context.Users.AnyAsync(u => u.Email == request.Email,cancellationToken);

            if (exists)
                throw new Exception("Email already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName
            };

           await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync(cancellationToken);

            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            _jobService.Enqueue<IEmailService>(service=>service.SendEmail(user.Email,user.FullName,"User Registration information", "Your account has been created successfully."))  ;

            return Result<AuthResponse>.Success(
                  new AuthResponse(
                      accessToken,
                      refreshToken,
                      3600,
                      new UserDto(user.Id, user.FullName, user.Email, user.Role)
                  ),
                  "User registered successfully"
              );
        }
    }
}
