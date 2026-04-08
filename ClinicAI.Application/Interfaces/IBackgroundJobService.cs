using System.Linq.Expressions;

namespace ClinicAI.Application.Interfaces
{
    public interface IBackgroundJobService
    {
        void Enqueue<T>(Expression<Func<T,Task>> job);
    }
}
