namespace BuckleApp.Application.Features.Appointments.CreateAppointment;

public interface ICreateAppointmentsService
{
    Task<IReadOnlyList<Guid>> ExecuteAsync(List<CreateAppointmentRequest> appointments);
}