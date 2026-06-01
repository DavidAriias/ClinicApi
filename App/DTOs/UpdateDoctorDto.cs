namespace ClinicApi.App.DTOs
{
    public class UpdateDoctorDto
    {
        public string Name { get; set; } = null!;
        public string Specialty { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}