namespace ClinicApi.App.DTOs
{
    public class UpdateAppointmentDto
    {
        public DateTime AppointmentDate { get; set; }
        public int DurationMinutes { get; set; }
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
    }
}