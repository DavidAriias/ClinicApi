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
        /// Get all appointments with pagination
        /// </summary>
        /// <remarks>
        /// Supports page and pageSize query parameters.
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

            return Ok(new
            {
                data = result.Data,
                total = result.Total,
                page = result.Page,
                pageSize = result.PageSize
            });
        }


        /// <summary>
        /// Get appointment by id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetById(id);

            if (result == null)
                return NotFound(new { message = "Appointment not found" });

            return Ok(result);
        }


        /// <summary>
        /// Create a new appointment
        /// </summary>
        /// <remarks>
        /// Business rules:
        /// - Must not overlap with other active appointments of same doctor
        /// - Doctor must be active
        /// - Appointment must be in the future
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(CreateAppointmentDto dto)
        {
            try
            {
                var result = await _service.Create(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Id },
                    result
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }


        /// <summary>
        /// Update an existing appointment
        /// </summary>
        /// <remarks>
        /// Cannot modify completed appointments.
        /// Validates schedule conflicts.
        /// </remarks>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(int id, UpdateAppointmentDto dto)
        {
            try
            {
                var result = await _service.Update(id, dto);

                if (!result)
                    return NotFound(new { message = "Appointment not found" });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cancel an appointment
        /// </summary>
        /// <remarks>
        /// Changes status to Cancelled instead of deleting.
        /// </remarks>
        [HttpPatch("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var result = await _service.Cancel(id);

                if (!result)
                    return NotFound(new { message = "Appointment not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}