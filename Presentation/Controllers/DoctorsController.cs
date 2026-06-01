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
        /// Get all doctors with pagination
        /// </summary>
        /// <remarks>
        /// Returns a paginated list of doctors.
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
        /// Get doctor by id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var doctor = await _service.GetById(id);

            if (doctor == null)
                return NotFound(new { message = "Doctor not found" });

            return Ok(doctor);
        }


        /// <summary>
        /// Create a new doctor
        /// </summary>
        /// <remarks>
        /// Name max length: 100 characters
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateDoctorDto dto)
        {
            try
            {
                var doctor = await _service.Create(dto.Name, dto.Specialty);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = doctor.Id },
                    doctor
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// <summary>
        /// Update an existing doctor
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, UpdateDoctorDto dto)
        {
            try
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
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a doctor by id
        /// </summary>
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