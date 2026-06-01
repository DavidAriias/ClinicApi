using ClinicApi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using ClinicApi.Domain.Entities;
using ClinicApi.Domain.Repositories;

namespace ClinicApi.Infraestructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Appointment> Query()
            => _context.Appointments.AsQueryable();

        public Task<Appointment?> GetById(int id)
            => _context.Appointments.FindAsync(id).AsTask();

        public Task Add(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            return _context.SaveChangesAsync();
        }

        public Task Update(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            return _context.SaveChangesAsync();
        }

        public Task Delete(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
            return _context.SaveChangesAsync();
        }

        public Task<List<Appointment>> GetByDoctor(int doctorId)
        {
            return _context.Appointments
                .Where(x => x.DoctorId == doctorId)
                .ToListAsync();
        }
    }
}