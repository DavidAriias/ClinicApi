using Microsoft.AspNetCore.Mvc;
using ClinicApi.App.Services;
using ClinicApi.App.DTOs;

namespace ClinicApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/doctors")]
    [Produces("application/json")]
    [Tags("Doctors")]
    public class DoctorsController : ControllerBase
    {
        private readonly DoctorService _service;

        public DoctorsController(DoctorService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves a paginated list of doctors with optional filters and sorting.
        /// </summary>
        /// <remarks>
        /// Supports filtering by name, specialty and active status.
        /// Also supports sorting by name or createdAt.
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            int page = 1,
            int pageSize = 10,
            string? name = null,
            string? specialty = null,
            bool? isActive = null,
            string sortBy = "name",
            string sortOrder = "asc")
        {
            var result = await _service.GetAll(
                page,
                pageSize,
                name,
                specialty,
                isActive,
                sortBy,
                sortOrder
            );

            return Ok(new
            {
                data = result.Data,
                total = result.Total,
                page = result.Page,
                pageSize = result.PageSize
            });
        }

        /// <summary>
        /// Retrieves a doctor by its unique identifier.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var doctor = await _service.GetById(id);

            if (doctor is null)
                return NotFound(new { message = "Doctor not found" });

            return Ok(doctor);
        }

        /// <summary>
        /// Creates a new doctor.
        /// </summary>
        /// <remarks>
        /// Business rules:
        /// - Name is required
        /// - Maximum length: 100 characters
        /// - Specialty is required
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateDoctorDto dto)
        {
            var doctor = await _service.Create(dto.Name, dto.Specialty);

            return CreatedAtAction(
                nameof(GetById),
                new { id = doctor.Id },
                doctor
            );
        }

        /// <summary>
        /// Updates an existing doctor.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, UpdateDoctorDto dto)
        {
            var result = await _service.Update(
                id,
                dto.Name,
                dto.Specialty,
                dto.IsActive
            );

            if (!result)
                return NotFound(new { message = "Doctor not found" });

            return NoContent();
        }

        /// <summary>
        /// Deletes a doctor by id.
        /// </summary>
        /// <remarks>
        /// A doctor cannot be deleted if it has future appointments.
        /// </remarks>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.Delete(id);

            if (!result)
                return NotFound(new { message = "Doctor not found" });

            return NoContent();
        }
    }
}