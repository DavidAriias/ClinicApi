using Microsoft.EntityFrameworkCore;
using ClinicApi.Domain.Entities;

namespace ClinicApi.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Specialty).IsRequired();
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.Property(x => x.PatientName).IsRequired();
                entity.Property(x => x.Status).IsRequired();

                entity.HasOne(x => x.Doctor)
                      .WithMany(x => x.Appointments)
                      .HasForeignKey(x => x.DoctorId);
            });
        }
    }
}