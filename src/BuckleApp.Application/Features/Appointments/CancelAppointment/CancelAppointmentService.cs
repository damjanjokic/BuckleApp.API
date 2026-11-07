using System.Net;
using BuckleApp.Application.Exceptions;
using BuckleApp.Application.Features.Appointments.CancelAppointment;
using BuckleApp.Core.IUnitOfWork;
using BuckleApp.Infrastructure.Interfaces;

namespace BuckleApp;

public class CancelAppointmentService : ICancelAppointmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserAccessor _userAccessor;
    
    public CancelAppointmentService(IUserAccessor userAccessor, IUnitOfWork unitOfWork)
    {
        _userAccessor = userAccessor;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> ExecuteAsync(Guid id, string reason)
    {
        var appointment = await _unitOfWork.Appointment.GetById(id, true);
        if (appointment == null)
        {
            throw new RestException(HttpStatusCode.NotFound, "Appointment not found");
        }

        var userAppointment =  appointment.UserAppointments.First();
        if (userAppointment == null)
        {
            throw new RestException(HttpStatusCode.NotFound, "Your are not part of this appointment");
        }
        
        userAppointment.UserCanceledId = _userAccessor.GetCurrentUserId();
        userAppointment.CancelationReason = reason;

        return Guid.Parse("38db990d-d53a-4795-3fc5-08dc9848a982");
    }
}