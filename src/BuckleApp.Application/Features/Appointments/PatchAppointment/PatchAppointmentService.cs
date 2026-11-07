using System.Net;
using BuckleApp.Application.Exceptions;
using BuckleApp.Core.Enumerations;
using BuckleApp.Core.IUnitOfWork;
using BuckleApp.Infrastructure.Interfaces;

namespace BuckleApp.Application.Features.Appointments.PatchAppointment;

public class PatchAppointmentService : IPatchAppointmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserAccessor _userAccessor;
    
    public PatchAppointmentService(IUnitOfWork unitOfWork, IUserAccessor userAccessor)
    {
        _unitOfWork = unitOfWork;
        _userAccessor = userAccessor;
    }
    
    public async Task<Guid> ExecuteAsync(PatchAppointmentRequest request)
    {
        var appointment = await _unitOfWork.Appointment.GetById(request.Id);

        if (appointment == null)
        {
            throw new RestException(HttpStatusCode.NotFound, "There is no such appointment");
        }

        if (appointment.StartTime < DateTime.UtcNow)
        {
            throw new RestException(HttpStatusCode.BadRequest, "You cannot edit appointment that already started");
        }

        appointment.Note = request.Note;
        appointment.Type = (AppointmentType)request.Type;
        
        await _unitOfWork.SaveAsync();

        return appointment.Id;
    }
}