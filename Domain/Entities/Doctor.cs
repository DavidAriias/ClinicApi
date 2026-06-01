namespace ClinicApi.Domain.Entities

{
    public class Doctor
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Specialty { get; set; } 
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Appointment> Appointments { get; set; } = [];
    }
}