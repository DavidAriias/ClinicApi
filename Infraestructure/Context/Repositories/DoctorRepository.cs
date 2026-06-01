using ClinicApi.Domain.Entities;
using ClinicApi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace ClinicApi.Domain.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly AppDbContext _context;

        public DoctorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Doctor>> GetAll()
        {
            return await _context.Doctors.ToListAsync();
        }

        public async Task<Doctor?> GetById(int id)
        {
            return await _context.Doctors.FindAsync(id);
        }

        public async Task Add(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Doctor doctor)
        {
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
        }
    }
}