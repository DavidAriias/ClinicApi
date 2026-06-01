using ClinicApi.Domain.Entities;
using ClinicApi.Domain.Repositories;
using ClinicApi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace ClinicApi.Infraestructure.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly AppDbContext _context;

        public DoctorRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Doctor> Query()
            => _context.Doctors.AsQueryable();

        public Task<Doctor?> GetById(int id)
            => _context.Doctors.FindAsync(id).AsTask();

        public Task Add(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            return _context.SaveChangesAsync();
        }

        public Task Update(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            return _context.SaveChangesAsync();
        }

        public async Task Delete(Doctor doctor)
        {
            doctor.IsActive = false;

            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();
        }
    }
}