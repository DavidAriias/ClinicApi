namespace ClinicApi.App.DTOs
{
    public class CreateDoctorDto
    {
        public required string Name { get; set; }
        public required string Specialty { get; set; }
    }
}