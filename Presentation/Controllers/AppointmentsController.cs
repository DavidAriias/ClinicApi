using Microsoft.AspNetCore.Mvc;
using ClinicApi.App.Services;
using ClinicApi.App.DTOs;

namespace ClinicApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    [Produces("application/json")]
    [Tags("Appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppointmentService _service;

        public AppointmentsController(AppointmentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves a paginated list of appointments with optional filters and sorting.
        /// </summary>
        /// <remarks>
        /// Supports filtering by doctor, status, patient name and date range.
        /// Also supports sorting by appointmentDate or createdAt.
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            int page = 1,
            int pageSize = 10,
            int? doctorId = null,
            string? status = null,
            string? patientName = null,
            DateTime? from = null,
            DateTime? to = null,
            string sortBy = "appointmentDate",
            string sortOrder = "asc"
        )
        {
            var result = await _service.GetAll(
                page,
                pageSize,
                doctorId,
                status,
                patientName,
                from,
                to,
                sortBy,
                sortOrder
            );

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific appointment by its unique identifier.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetById(id);

            if (result is null)
                return NotFound(new { message = "Appointment not found" });

            return Ok(result);
        }

        /// <summary>
        /// Creates a new appointment.
        /// </summary>
        /// <remarks>
        /// Business rules enforced:
        /// - No overlapping appointments for the same doctor
        /// - Doctor must be active
        /// - Appointment must be scheduled in the future
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(CreateAppointmentDto dto)
        {
            var result = await _service.Create(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result
            );
        }

        /// <summary>
        /// Updates an existing appointment.
        /// </summary>
        /// <remarks>
        /// Cannot modify completed appointments and validates schedule conflicts.
        /// </remarks>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, UpdateAppointmentDto dto)
        {
            var result = await _service.Update(id, dto);

            if (!result)
                return NotFound(new { message = "Appointment not found" });

            return NoContent();
        }

        /// <summary>
        /// Cancels an existing appointment.
        /// </summary>
        /// <remarks>
        /// The appointment is not deleted, only its status is set to Cancelled.
        /// </remarks>
        [HttpPatch("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _service.Cancel(id);

            if (!result)
                return NotFound(new { message = "Appointment not found" });

            return NoContent();
        }
    }
}