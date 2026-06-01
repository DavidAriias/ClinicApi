using Microsoft.AspNetCore.Mvc;
using ClinicApi.App.Services;
using ClinicApi.App.DTOs;

namespace ClinicApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/doctors")]
    public class DoctorsController : ControllerBase
    {
        private readonly DoctorService _service;

        public DoctorsController(DoctorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAll());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var doctor = await _service.GetById(id);

            if (doctor == null)
                return NotFound(new { message = "Doctor not found" });

            return Ok(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDoctorDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { message = "Name is required" });

            var doctor = await _service.Create(dto.Name, dto.Specialty);

            return CreatedAtAction(
                nameof(GetById),
                new { id = doctor.Id },
                doctor
            );
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateDoctorDto dto)
        {
            var result = await _service.Update(id, dto.Name, dto.Specialty, dto.IsActive);

            if (!result)
                return NotFound(new { message = "Doctor not found" });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.Delete(id);

            if (!result)
                return NotFound(new { message = "Doctor not found" });

            return NoContent();
        }
    }
}