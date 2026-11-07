using BuckleApp.Application.Dtos;

namespace BuckleApp.Application.Features.Appointments.GetAppointmentsByOrganiserAndDate;

public interface IGetAppointmentsByOrganiserAndDateService
{
    Task<IReadOnlyList<GetAppointmentInfoDto>> ExecuteAsync(Guid organiserId, List<DateTime> dates);
}