namespace BuckleApp.Application.Features.Appointments.GetUpcomingForUserByDate;

public interface IGetUpcomingForUserByDateService
{
    Task<List<UpcomingAppointmentDto>> ExecuteAsync(DateTime date);
}