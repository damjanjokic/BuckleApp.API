namespace BuckleApp.Application.Features.Appointments.DeleteAppointment;

public interface IDeleteAppointmentService
{
    Task<Guid> ExecuteAsync(Guid id);
}