namespace BuckleApp.Application.Features.Appointments.CancelAppointment;

public interface ICancelAppointmentService
{
    Task<Guid> ExecuteAsync(Guid id, string reason);
}