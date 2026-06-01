using ClinicApi.Domain.Enums;

namespace ClinicApi.Domain.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public string PatientName { get; set; } = null!;
        public DateTime AppointmentDate { get; set; }
        public int DurationMinutes { get; set; }

        public string Status { get; set; } = AppointmentStatus.Scheduled;
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}