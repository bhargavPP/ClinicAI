using ClinicAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Application.Interfaces
{
    public interface IClinicDbContext
    {
        DbSet<Doctor> Doctors { get; }
        DbSet<Patient> Patients { get; }
        DbSet<Appointment> Appointments { get; }

        DbSet<User> Users { get; }
        DbSet<DoctorsAvailability> DoctorsAvailabilities { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
