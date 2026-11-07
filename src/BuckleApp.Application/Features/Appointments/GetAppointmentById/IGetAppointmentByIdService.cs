namespace BuckleApp.Application.Features.Appointments.GetAppointmentById;

public interface IGetAppointmentByIdService
{
    Task<GetAppointmentByIdResponse> ExecuteAsync(Guid id);
}