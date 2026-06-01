using ClinicApi.Domain.Entities;

namespace ClinicApi.Domain.Repositories
{
    public interface IDoctorRepository
    {
        IQueryable<Doctor> Query();
        Task<Doctor?> GetById(int id);
        Task Add(Doctor doctor);
        Task Update(Doctor doctor);
        Task Delete(Doctor doctor);
    }
}