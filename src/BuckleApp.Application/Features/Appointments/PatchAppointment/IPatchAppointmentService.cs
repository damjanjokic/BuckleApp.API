namespace BuckleApp.Application.Features.Appointments.PatchAppointment;

public interface IPatchAppointmentService
{
    Task<Guid> ExecuteAsync(PatchAppointmentRequest request);
}