using ClinicApi.Domain.Entities;
using ClinicApi.Domain.Repositories;

namespace ClinicApi.App.Services
{
    public class DoctorService
    {
        private readonly IDoctorRepository _repo;

        public DoctorService(IDoctorRepository repo)
        {
            _repo = repo;
        }

        public Task<List<Doctor>> GetAll()
            => _repo.GetAll();

        public Task<Doctor?> GetById(int id)
            => _repo.GetById(id);

        public async Task<Doctor> Create(string name, string specialty)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required");

            if (string.IsNullOrWhiteSpace(specialty))
                throw new ArgumentException("Specialty is required");

            var doctor = new Doctor
            {
                Name = name,
                Specialty = specialty,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _repo.Add(doctor);
            return doctor;
        }

        public async Task<bool> Update(int id, string name, string specialty, bool isActive)
        {
            var doctor = await _repo.GetById(id);

            if (doctor == null)
                return false;

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required");

            if (string.IsNullOrWhiteSpace(specialty))
                throw new ArgumentException("Specialty is required");

            doctor.Name = name;
            doctor.Specialty = specialty;
            doctor.IsActive = isActive;

            await _repo.Update(doctor);
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var doctor = await _repo.GetById(id);

            if (doctor == null)
                return false;

            await _repo.Delete(doctor);
            return true;
        }
    }
}