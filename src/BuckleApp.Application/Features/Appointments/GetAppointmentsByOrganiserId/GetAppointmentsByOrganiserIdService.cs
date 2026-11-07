using BuckleApp.Application.Dtos;
using BuckleApp.Core.IUnitOfWork;
using BuckleApp.Infrastructure.Interfaces;
using Mapster;

namespace BuckleApp.Application.Features.Appointments.GetAppointmentsByOrganiserId;

public class GetAppointmentsByOrganiserIdService : IGetAppointmentsByOrganiserIdService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserAccessor _userAccessor;
    
    public GetAppointmentsByOrganiserIdService(IUnitOfWork unitOfWork, IUserAccessor userAccessor)
    {
        _unitOfWork = unitOfWork;
        _userAccessor = userAccessor;
    }
    
    public async Task<IReadOnlyList<GetAppointmentDto>> ExecuteAsync(Guid organiserId)
    {
        var appointments = await _unitOfWork.Appointment.GetAllByOrganiserId(organiserId);

        return appointments.Adapt<List<GetAppointmentDto>>();
    }
}