using ClinicApi.Domain.Entities;

namespace ClinicApi.Domain.Repositories
{
    public interface IDoctorRepository
    {
        Task<List<Doctor>> GetAll();
        Task<Doctor?> GetById(int id);
        Task Add(Doctor doctor);
        Task Update(Doctor doctor);
        Task Delete(Doctor doctor);
    }
}