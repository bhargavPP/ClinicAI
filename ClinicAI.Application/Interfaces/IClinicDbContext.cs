using ClinicAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Interfaces
{
    public interface IClinicDbContext
    {
        DbSet<Doctor> Doctors { get; }
        DbSet<Patient> Patients { get; }
        DbSet<Appointment> Appointments { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
