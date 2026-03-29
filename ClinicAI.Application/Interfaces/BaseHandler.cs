namespace ClinicAI.Application.Interfaces
{
    public abstract class BaseHandler
    {
        protected readonly IClinicDbContext _context;
        protected readonly ICurrentUserService _currentUser;
        protected readonly IDateTime _dateTime;
        protected BaseHandler(
            IClinicDbContext context,
            ICurrentUserService currentUser,
            IDateTime dateTime  )
        {
            _context = context;
            _currentUser = currentUser;
            _dateTime = dateTime;
        }

        protected Guid currentUserId => _currentUser.UserId;
    }
}
