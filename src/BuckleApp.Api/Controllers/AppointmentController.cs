using BuckleApp.Application.Features.Appointments;
using BuckleApp.Application.Features.Appointments.CancelAppointment;
using BuckleApp.Application.Features.Appointments.CreateAppointment;
using BuckleApp.Application.Features.Appointments.PatchAppointment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuckleApp.API.Controllers;

//[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentsService _appointmentsService;

    public AppointmentController(IAppointmentsService appointmentsService)
    {
        _appointmentsService = appointmentsService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointmentById(Guid id)
    {
        var response = await _appointmentsService.GetByIdAsync(id);
        return Ok(response);
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateAppointment([FromBody] List<CreateAppointmentRequest> request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var response = await _appointmentsService.CreateAsync(request);

        return Ok(response);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchAppointment(Guid id, [FromBody] PatchAppointmentRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        request.Id = id;
        var response = await _appointmentsService.PatchAsync(request);
    
        return Ok(response);
    }
    
    [HttpGet("organiser/{organiserId}")]
    public async Task<IActionResult> GetAppointmentsByOrganiserIdAndDates(Guid organiserId, [FromQuery] List<DateTime> dates)
    {
        var response = await _appointmentsService.GetByOrganiserAndDatesAsync(organiserId, dates);
        return Ok(response);
    }

    [HttpPatch("cancelAppointment/{id}")]
    public async Task<IActionResult> CancelAppointment(Guid id, [FromBody] CancelAppointmentRequest request)
    {
        if (!ModelState.IsValid) return BadRequest();

        var response = await _appointmentsService.CancelAsync(id, request.Reason);
        return Ok(response);
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(Guid id)
    {
        var response = await _appointmentsService.DeleteAsync(id);

        return Ok(response);
    }
}