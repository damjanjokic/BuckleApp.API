using BuckleApp.Application.Dtos;
using BuckleApp.Application.Features.Appointments.CreateAppointment;
using BuckleApp.Application.Features.Appointments.GetAppointmentById;
using BuckleApp.Application.Features.Appointments.PatchAppointment;

namespace BuckleApp.Application.Features.Appointments;

public interface IAppointmentsService
{
    Task<GetAppointmentByIdResponse> GetByIdAsync(Guid id);

    Task<IReadOnlyList<Guid>> CreateAsync(
        List<CreateAppointmentRequest> appointments);

    Task<Guid> PatchAsync(
        PatchAppointmentRequest request);

    Task<IReadOnlyList<GetAppointmentInfoDto>> GetByOrganiserAndDatesAsync(
        Guid organiserId,
        List<DateTime> dates);

    Task<Guid> CancelAsync(
        Guid id, string reason);

    Task<Guid> DeleteAsync(Guid id);
}