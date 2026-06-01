using ClinicApi.Domain.Entities;
using ClinicApi.Domain.Repositories;
using ClinicApi.Domain.Enums;
using ClinicApi.App.Common;
using ClinicApi.App.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ClinicApi.App.Services
{
    public class AppointmentService
    {
        private readonly IAppointmentRepository _repo;
        private readonly IDoctorRepository _doctorRepo;

        public AppointmentService(
            IAppointmentRepository repo,
            IDoctorRepository doctorRepo)
        {
            _repo = repo;
            _doctorRepo = doctorRepo;
        }


        public async Task<PagedResult<Appointment>> GetAll(
             int page = 1,
             int pageSize = 10,
             int? doctorId = null,
             string? status = null,
             string? patientName = null,
             DateTime? from = null,
             DateTime? to = null,
             string sortBy = "appointmentDate",
             string sortOrder = "asc")
        {
            pageSize = pageSize > 50 ? 50 : pageSize;

            var query = _repo.Query();

            if (doctorId.HasValue)
                query = query.Where(x => x.DoctorId == doctorId);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            if (!string.IsNullOrWhiteSpace(patientName))
                query = query.Where(x => x.PatientName.Contains(patientName));

            if (from.HasValue)
                query = query.Where(x => x.AppointmentDate >= from);

            if (to.HasValue)
                query = query.Where(x => x.AppointmentDate <= to);

            var total = await query.CountAsync();


            query = (sortBy.ToLower(), sortOrder.ToLower()) switch
            {
                ("createdat", "desc") => query.OrderByDescending(x => x.CreatedAt),
                ("createdat", _) => query.OrderBy(x => x.CreatedAt),

                ("appointmentdate", "desc") => query.OrderByDescending(x => x.AppointmentDate),
                _ => query.OrderBy(x => x.AppointmentDate)
            };


            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Appointment>
            {
                Data = data,
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }


        public Task<Appointment?> GetById(int id)
            => _repo.GetById(id);


        public async Task<Appointment> Create(CreateAppointmentDto dto)
        {
            if (dto.AppointmentDate <= DateTime.UtcNow)
                throw new ArgumentException("AppointmentDate must be in the future");

            if (dto.DurationMinutes <= 0)
                throw new ArgumentException("Duration must be greater than 0");

            var doctor = await _doctorRepo.GetById(dto.DoctorId);

            if (doctor == null || !doctor.IsActive)
                throw new Exception("Doctor not available");

            var existing = await _repo.GetByDoctor(dto.DoctorId);

            var newStart = dto.AppointmentDate;
            var newEnd = dto.AppointmentDate.AddMinutes(dto.DurationMinutes);

            if (HasOverlap(existing, newStart, newEnd, dto.DoctorId))
                throw new Exception("Appointment overlaps with another appointment");

            var appointment = new Appointment
            {
                DoctorId = dto.DoctorId,
                PatientName = dto.PatientName,
                AppointmentDate = dto.AppointmentDate,
                DurationMinutes = dto.DurationMinutes,
                Status = AppointmentStatus.Scheduled,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.Add(appointment);
            return appointment;
        }

        public async Task<bool> Update(int id, UpdateAppointmentDto dto)
        {
            var appointment = await _repo.GetById(id);

            if (appointment == null)
                return false;

            if (appointment.Status == AppointmentStatus.Completed)
                throw new Exception("Cannot modify completed appointment");

            var doctor = await _doctorRepo.GetById(dto.DoctorId);

            if (doctor == null || !doctor.IsActive)
                throw new Exception("Doctor not available");

            var existing = await _repo.GetByDoctor(dto.DoctorId);

            var newStart = dto.AppointmentDate;
            var newEnd = dto.AppointmentDate.AddMinutes(dto.DurationMinutes);

            if (HasOverlap(existing, newStart, newEnd, dto.DoctorId, id))
                throw new Exception("Appointment overlaps with another appointment");

            appointment.DoctorId = dto.DoctorId;
            appointment.PatientName = dto.PatientName;
            appointment.AppointmentDate = dto.AppointmentDate;
            appointment.DurationMinutes = dto.DurationMinutes;
            appointment.Notes = dto.Notes;

            await _repo.Update(appointment);

            return true;
        }

        public async Task<bool> Cancel(int id)
        {
            var doctor = await _repo.GetById(id);

            if (doctor == null)
                return false;

            var appointments = await _appointmentRepo.GetByDoctor(id);

            var hasFutureAppointments = appointments.Any(a =>
                a.Status == AppointmentStatus.Scheduled &&
                a.AppointmentDate > DateTime.UtcNow
            );

            if (hasFutureAppointments)
                throw new Exception("Doctor has future appointments");

            await _repo.Delete(doctor);
            return true;
        }

        private bool HasOverlap(
            List<Appointment> appointments,
            DateTime newStart,
            DateTime newEnd,
            int doctorId,
            int? ignoreAppointmentId = null)
        {
            return appointments.Any(a =>
                a.DoctorId == doctorId &&
                a.Status != AppointmentStatus.Cancelled &&
                (ignoreAppointmentId == null || a.Id != ignoreAppointmentId) &&
                newStart < a.AppointmentDate.AddMinutes(a.DurationMinutes) &&
                newEnd > a.AppointmentDate
            );
        }
    }
}