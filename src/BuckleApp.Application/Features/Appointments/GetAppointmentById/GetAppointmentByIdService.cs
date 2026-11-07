using System.Net;
using BuckleApp.Application.Exceptions;
using BuckleApp.Core.IUnitOfWork;
using BuckleApp.Infrastructure.Interfaces;
using Mapster;

namespace BuckleApp.Application.Features.Appointments.GetAppointmentById;

public class GetAppointmentByIdService : IGetAppointmentByIdService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserAccessor _userAccessor;

    public GetAppointmentByIdService(IUnitOfWork unitOfWork, IUserAccessor userAccessor)
    {
        _unitOfWork = unitOfWork;
        _userAccessor = userAccessor;
    }
    
    public async Task<GetAppointmentByIdResponse> ExecuteAsync(Guid id)
    {
        var appointment = await _unitOfWork.Appointment.GetById(id, true);
        if (appointment == null)
        {
            throw new RestException(HttpStatusCode.NotFound, "There is no such appointment");
        }

        var response = appointment.Adapt<GetAppointmentByIdResponse>();
        return response;
    }
}