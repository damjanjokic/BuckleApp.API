using BuckleApp.Application.Dtos;
using BuckleApp.Application.Features.Appointments.CancelAppointment;
using BuckleApp.Application.Features.Appointments.CreateAppointment;
using BuckleApp.Application.Features.Appointments.DeleteAppointment;
using BuckleApp.Application.Features.Appointments.GetAppointmentById;
using BuckleApp.Application.Features.Appointments.GetAppointmentsByOrganiserAndDate;
using BuckleApp.Application.Features.Appointments.PatchAppointment;

namespace BuckleApp.Application.Features.Appointments;

public class AppointmentsService : IAppointmentsService
{
    private readonly IGetAppointmentByIdService _getById;
    private readonly ICreateAppointmentsService _create;
    private readonly IPatchAppointmentService _patch;
    private readonly IGetAppointmentsByOrganiserAndDateService _getByOrganiserAndDates;
    private readonly ICancelAppointmentService _cancel;
    private readonly IDeleteAppointmentService _delete;

    public AppointmentsService(
        IGetAppointmentByIdService getById,
        ICreateAppointmentsService create,
        IPatchAppointmentService patch,
        IGetAppointmentsByOrganiserAndDateService getByOrganiserAndDates,
        ICancelAppointmentService cancel,
        IDeleteAppointmentService delete)
    {
        _getById = getById;
        _create = create;
        _patch = patch;
        _getByOrganiserAndDates = getByOrganiserAndDates;
        _cancel = cancel;
        _delete = delete;
    }
    
    public async Task<GetAppointmentByIdResponse> GetByIdAsync(Guid id)
        => await _getById.ExecuteAsync(id);

    public async Task<IReadOnlyList<Guid>> CreateAsync(List<CreateAppointmentRequest> appointments)
        => await _create.ExecuteAsync(appointments);

    public async Task<Guid> PatchAsync(PatchAppointmentRequest request)
        => await _patch.ExecuteAsync(request);

    public async Task<IReadOnlyList<GetAppointmentInfoDto>> GetByOrganiserAndDatesAsync(Guid organiserId, List<DateTime> dates)
        => await _getByOrganiserAndDates.ExecuteAsync(organiserId, dates);

    public async Task<Guid> CancelAsync(Guid id, string reason)
        => await _cancel.ExecuteAsync(id, reason);

    public async Task<Guid> DeleteAsync(Guid id)
        => await _delete.ExecuteAsync(id);
}