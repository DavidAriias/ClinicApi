using ClinicApi.Domain.Entities;
using ClinicApi.Domain.Repositories;
using ClinicApi.App.Common;

namespace ClinicApi.App.Services
{
    public class DoctorService
    {
        private readonly IDoctorRepository _repo;

        public DoctorService(IDoctorRepository repo)
        {
            _repo = repo;
        }

       
        public async Task<PagedResult<Doctor>> GetAll(int page = 1, int pageSize = 10)
        {
            pageSize = pageSize > 50 ? 50 : pageSize;

            var query = _repo.Query();

            var total = query.Count();

            var data = query
                .OrderBy(x => x.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<Doctor>
            {
                Data = data,
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

      
        public Task<Doctor?> GetById(int id)
            => _repo.GetById(id);

      
        public async Task<Doctor> Create(string name, string specialty)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required");

            if (name.Length > 100)
                throw new ArgumentException("Name max length is 100");

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

            if (name.Length > 100)
                throw new ArgumentException("Name max length is 100");

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