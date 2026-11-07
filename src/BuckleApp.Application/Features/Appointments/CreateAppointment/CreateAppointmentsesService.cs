using System.Net;
using BuckleApp.Application.Exceptions;
using BuckleApp.Core.Entities;
using BuckleApp.Core.IUnitOfWork;
using BuckleApp.Infrastructure.Interfaces;
using Mapster;

namespace BuckleApp.Application.Features.Appointments.CreateAppointment;

public class CreateAppointmentsService : ICreateAppointmentsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserAccessor _userAccessor;
    
    public CreateAppointmentsService(IUnitOfWork unitOfWork, IUserAccessor userAccessor)
    {
        _unitOfWork = unitOfWork;
        _userAccessor = userAccessor;
    }
    
    public async Task<IReadOnlyList<Guid>> ExecuteAsync(List<CreateAppointmentRequest> request)
    {
        //var userId = _userAccessor.GetCurrentUserId();
        var appointments = request.Adapt<List<Appointment>>();
        foreach (var appointment in appointments)
        {
            appointment.UserId = _userAccessor.GetCurrentUserId();
            
            var overlappingAppointment = await 
                _unitOfWork.Appointment.OverlappingAppointmentAsync(appointment.StartTime, appointment.EndTime,
                    appointment.UserId);
            
            if (overlappingAppointment != null)
            {
                throw new RestException(HttpStatusCode.BadRequest, "Appointment is overlapping");
            }
        }

        await _unitOfWork.Appointment.CreateAppointments(appointments);
        await _unitOfWork.SaveAsync();

        return appointments.Select(x => x.Id).ToList();
    }
}