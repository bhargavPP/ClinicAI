using ClinicAI.Application.Interfaces;
using ClinicAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Infrastructure.Persistence
{
    public class ClinicDbContext : DbContext, IClinicDbContext
    {
        public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options)
        {

        }
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<User> Users => Set<User>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
                entity.HasKey(d => d.id);
                entity.Property(a => a.Email).IsRequired();
                entity.Property(a => a.PasswordHash).IsRequired();

                entity.HasMany(u => u.Patients)
                      .WithOne(p => p.User)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
