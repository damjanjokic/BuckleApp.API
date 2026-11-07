using BuckleApp.Application.Dtos;
using BuckleApp.Application.Extensions;
using BuckleApp.Core.IUnitOfWork;
using BuckleApp.Infrastructure.Interfaces;
using Mapster;
using Microsoft.Extensions.Configuration;

namespace BuckleApp.Application.Features.Appointments.GetAppointmentsByOrganiserAndDate;

public class GetAppointmentsByOrganiserAndDateService : IGetAppointmentsByOrganiserAndDateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserAccessor _userAccessor;
    private readonly IConfiguration _configuration;

    public GetAppointmentsByOrganiserAndDateService(IUnitOfWork unitOfWork, IUserAccessor userAccessor, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _userAccessor = userAccessor;
        _configuration = configuration;
    }

    public async Task<IReadOnlyList<GetAppointmentInfoDto>> ExecuteAsync(Guid organiserId, List<DateTime> dates)
    {
        var onlyDates = dates.Select(x => x.Date.Date).ToList();
        var appointments = await _unitOfWork.Appointment.GetAllByOrganiserIdAndDates(organiserId, onlyDates);
        var timeZone = _configuration["TimeZone"];
        foreach (var appointment in appointments)
        {
            appointment.StartTime = appointment.StartTime.ConvertToTimeZone(timeZone);
            appointment.EndTime = appointment.EndTime.ConvertToTimeZone(timeZone);
        }
        
        return appointments.Adapt<List<GetAppointmentInfoDto>>();
    }
}