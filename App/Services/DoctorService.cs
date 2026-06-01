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


        public async Task<PagedResult<Doctor>> GetAll(
              int page = 1,
              int pageSize = 10,
              string? name = null,
              string? specialty = null,
              bool? isActive = null,
              string sortBy = "name",
              string sortOrder = "asc")
        {
            pageSize = pageSize > 50 ? 50 : pageSize;

            var query = _repo.Query();


            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(x => x.Name.Contains(name));

            if (!string.IsNullOrWhiteSpace(specialty))
                query = query.Where(x => x.Specialty.Contains(specialty));

            if (isActive.HasValue)
                query = query.Where(x => x.IsActive == isActive);


            var total = query.Count();

            query = (sortBy.ToLower(), sortOrder.ToLower()) switch
            {
                ("specialty", "desc") => query.OrderByDescending(x => x.Specialty),
                ("specialty", _) => query.OrderBy(x => x.Specialty),

                ("createdat", "desc") => query.OrderByDescending(x => x.CreatedAt),
                ("createdat", _) => query.OrderBy(x => x.CreatedAt),

                ("name", "desc") => query.OrderByDescending(x => x.Name),
                _ => query.OrderBy(x => x.Name)
            };


            var data = query
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