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
        //  private readonly IBackgroundJobService _jobService;
        private readonly IQueueService _queueService;
        public RegisterUserHandler(IClinicDbContext context, IJwtTokenService jwtTokenService, IQueueService queueService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
            _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
        }
        public async Task<Result<AuthResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (request.Password != request.ConfirmPassword)
                throw new Exception("Passwords do not match");

            var exists = await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);

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


            // _jobService.Enqueue<IEmailService>(service=>service.SendEmail(user.Email,user.FullName,"User Registration information", "Your account has been created successfully."))  ;

            var emailMessage = new EmailMessage
            {
                ToEmail = user.Email,
                Subject = "User Registration information",
                Body = $"Hello {user.FullName}, your account has been created successfully."
            };
            await _queueService.EnqueuAsync(emailMessage);

            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

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
