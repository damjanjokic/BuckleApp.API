using BuckleApp.Core.IUnitOfWork;
using BuckleApp.Infrastructure.Interfaces;
using Mapster;

namespace BuckleApp.Application.Features.Appointments.GetUpcomingForUserByDate;

public class GetUpcomingForUserByDateService : IGetUpcomingForUserByDateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserAccessor _userAccessor;

    public GetUpcomingForUserByDateService(IUnitOfWork unitOfWork, IUserAccessor userAccessor)
    {
        _unitOfWork = unitOfWork;
        _userAccessor = userAccessor;
    }
    
    public async Task<List<UpcomingAppointmentDto>> ExecuteAsync(DateTime date)
    {
        var userId = _userAccessor.GetCurrentUserId();
        var appointments = await _unitOfWork.Appointment.GetUpcomingForUserByDate(userId, date);
        
        return appointments.Adapt<List<UpcomingAppointmentDto>>();
    }
}