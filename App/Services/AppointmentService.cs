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
            int pageSize = 10)
        {
            pageSize = pageSize > 50 ? 50 : pageSize;

            var query = _repo.Query();

            var total = await query.CountAsync();

            var data = await query
                .OrderBy(x => x.AppointmentDate)
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
            if (string.IsNullOrWhiteSpace(dto.PatientName))
                throw new ArgumentException("PatientName is required");

            if (dto.DurationMinutes <= 0)
                throw new ArgumentException("Duration must be greater than 0");

            if (dto.AppointmentDate <= DateTime.UtcNow)
                throw new ArgumentException("Appointment must be in the future");

            var doctor = await _doctorRepo.GetById(dto.DoctorId) ?? throw new Exception("Doctor not found");

            if (!doctor.IsActive)
                throw new Exception("Doctor is not active");

            var appointments = await _repo.Query()
                .Where(x => x.DoctorId == dto.DoctorId)
                .ToListAsync();

            var newStart = dto.AppointmentDate;
            var newEnd = dto.AppointmentDate.AddMinutes(dto.DurationMinutes);

            var hasOverlap = appointments.Any(a =>
            {
                if (a.Status == AppointmentStatus.Cancelled)
                    return false;

                var start = a.AppointmentDate;
                var end = a.AppointmentDate.AddMinutes(a.DurationMinutes);

                return newStart < end && newEnd > start;
            });

            if (hasOverlap)
                throw new Exception("Time slot not available");

            var appointment = new Appointment
            {
                DoctorId = dto.DoctorId,
                PatientName = dto.PatientName,
                AppointmentDate = dto.AppointmentDate,
                DurationMinutes = dto.DurationMinutes,
                Notes = dto.Notes,
                Status = AppointmentStatus.Scheduled,
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
                throw new Exception("Cannot modify a completed appointment");

            if (!AppointmentStatus.IsValid(dto.Status))
                throw new ArgumentException("Invalid status");

            var appointments = await _repo.Query()
                .Where(x => x.DoctorId == appointment.DoctorId)
                .ToListAsync();

            var newStart = dto.AppointmentDate;
            var newEnd = dto.AppointmentDate.AddMinutes(dto.DurationMinutes);

            var hasOverlap = appointments.Any(a =>
            {
                if (a.Id == appointment.Id)
                    return false;

                if (a.Status == AppointmentStatus.Cancelled)
                    return false;

                var start = a.AppointmentDate;
                var end = a.AppointmentDate.AddMinutes(a.DurationMinutes);

                return newStart < end && newEnd > start;
            });

            if (hasOverlap)
                throw new Exception("Time slot not available");

            appointment.AppointmentDate = dto.AppointmentDate;
            appointment.DurationMinutes = dto.DurationMinutes;
            appointment.Status = dto.Status;
            appointment.Notes = dto.Notes;

            await _repo.Update(appointment);
            return true;
        }

 
        public async Task<bool> Cancel(int id)
        {
            var appointment = await _repo.GetById(id);

            if (appointment == null)
                return false;

            if (appointment.Status == AppointmentStatus.Completed)
                throw new Exception("Cannot cancel a completed appointment");

            appointment.Status = AppointmentStatus.Cancelled;

            await _repo.Update(appointment);
            return true;
        }
    }
}