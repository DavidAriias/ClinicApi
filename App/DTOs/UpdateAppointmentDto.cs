namespace ClinicApi.App.DTOs
{
    public class UpdateAppointmentDto
    {
        public int DoctorId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public int DurationMinutes { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}