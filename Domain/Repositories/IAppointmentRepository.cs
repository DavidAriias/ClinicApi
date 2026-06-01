using ClinicApi.Domain.Entities;

namespace ClinicApi.Domain.Repositories
{
    public interface IAppointmentRepository
    {
        IQueryable<Appointment> Query();

        Task<Appointment?> GetById(int id);
        Task Add(Appointment appointment);
        Task Update(Appointment appointment);
        Task Delete(Appointment appointment);
        Task<List<Appointment>> GetByDoctor(int doctorId);
    }
}