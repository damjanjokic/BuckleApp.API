using System.Net;
using BuckleApp.Application.Exceptions;
using BuckleApp.Core.IUnitOfWork;
using BuckleApp.Infrastructure.Interfaces;

namespace BuckleApp.Application.Features.Appointments.DeleteAppointment;

public class DeleteAppointmentService : IDeleteAppointmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserAccessor _userAccessor;

    public DeleteAppointmentService(IUnitOfWork unitOfWork, IUserAccessor userAccessor)
    {
        _unitOfWork = unitOfWork;
        _userAccessor = userAccessor;
    }

    public async Task<Guid> ExecuteAsync(Guid id)
    {
        var appointment = await _unitOfWork.Appointment.GetById(id, true);

        if (appointment == null)
        {
            throw new RestException(HttpStatusCode.NotFound, "Appointment not found");
        }

        if (appointment.UserAppointments.Any())
        {
            throw new RestException(HttpStatusCode.BadRequest, "Appointment with attendee cannot be deleted");
        }
        
        _unitOfWork.Appointment.Remove(appointment);
        await _unitOfWork.SaveAsync();

        return id;
    }
}