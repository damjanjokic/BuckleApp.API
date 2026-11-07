using BuckleApp.Application.Dtos;

namespace BuckleApp.Application.Features.Appointments.GetAppointmentsByOrganiserId;

public interface IGetAppointmentsByOrganiserIdService
{
    Task<IReadOnlyList<GetAppointmentDto>> ExecuteAsync(Guid organiserId);
}