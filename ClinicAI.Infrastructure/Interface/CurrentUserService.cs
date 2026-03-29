using ClinicAI.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ClinicAI.Infrastructure.Interface
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var userId = _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                    return Guid.Empty;

                return Guid.Parse(userId);
            }
        }
    }
}
