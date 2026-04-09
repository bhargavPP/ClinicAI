using ClinicAI.Application.Interfaces;
using ClinicAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicAI.Infrastructure.Persistence
{
    public class ClinicDbContext : DbContext, IClinicDbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;
        public ClinicDbContext(DbContextOptions<ClinicDbContext> options, ICurrentUserService currentUserService,IDateTime dateTime ) : base(options)
        {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _dateTime = dateTime;
        }
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<DoctorsAvailability> DoctorsAvailabilities => Set<DoctorsAvailability>();
        public DbSet<EmailLog> EmailLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Appointment>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<DoctorsAvailability>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Doctor>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(a => a.Name).IsRequired().HasMaxLength(150);
                entity.Property(a => a.Email).IsRequired();
                entity.Property(a => a.Phone).IsRequired();
                entity.Property(a => a.Specialization).IsRequired();

            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(a => a.Name).IsRequired().HasMaxLength(150);
                entity.Property(a => a.Email).IsRequired();
                entity.Property(a => a.Phone).IsRequired();
                entity.Property(a => a.DateOfBirth).IsRequired();
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(d => d.Id);

                entity.Property(a => a.AppointmentDate).IsRequired();
                entity.Property(a => a.StartTime).IsRequired();
                entity.Property(a => a.EndTime).IsRequired();
                entity.Property(a => a.Status).IsRequired().HasMaxLength(50);

                entity.HasOne(a => a.Doctor)
                      .WithMany(d => d.Appointments)
                      .HasForeignKey(a => a.DoctorId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(a => a.Patient)
                      .WithMany(p => p.Appointments)
                      .HasForeignKey(a => a.PatientId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(a => a.Email).IsRequired();
                entity.Property(a => a.PasswordHash).IsRequired();

                entity.HasMany(u => u.Patients)
                      .WithOne(p => p.User)
                      .HasForeignKey(p => p.UserId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DoctorsAvailability>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(a => a.DoctorId).IsRequired();
                entity.Property(a => a.Date).IsRequired();
                entity.Property(a => a.StartTime).IsRequired();
                entity.Property(a => a.EndTime).IsRequired();
                entity.Property(a => a.IsAvailable).IsRequired();
                entity.HasOne(da => da.Doctor)
                      .WithMany(d => d.DoctorsAvailabilities)
                      .HasForeignKey(da => da.DoctorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<EmailLog>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.UserId);         // fast lookup by user
                entity.HasIndex(e => e.Status);         // fast lookup by status
                entity.HasIndex(e => e.CorrelationId);  // fast lookup by correlation
                entity.HasIndex(e => e.CreatedAt);      // fast lookup by date

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.SetNull); // keep logs even if user deleted

                entity.Property(e => e.Type)
                      .HasConversion<string>(); // store as string not int

                entity.Property(e => e.Status)
                      .HasConversion<string>();
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.UserId;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = _dateTime.dateTimeUtcNow; 
                    if (userId != Guid.Empty)
                        entry.Entity.CreatedBy = userId;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = _dateTime.dateTimeUtcNow;
                    if (userId != Guid.Empty)
                        entry.Entity.UpdatedBy = userId;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
