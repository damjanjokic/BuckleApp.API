// using BuckleApp.Application.Extensions;
// using BuckleApp.Application.Dtos;
// using BuckleApp.Core.IUnitOfWork;
// using BuckleApp.Infrastructure.Interfaces;
// using Mapster;
// using MediatR;
// using Microsoft.Extensions.Configuration;
//
// namespace BuckleApp.Application.Features.Appointments.GetAppointmentsByOrganiserAndDate;
//
// public class GetByOrganiserAndDateHandler : IRequestHandler<GetByOrganiserAndDateQuery, GetByOrganiserAndDateResponse>
// {
//     private readonly IUnitOfWork _unitOfWork;
//     private readonly IUserAccessor _userAccessor;
//     private readonly IConfiguration _configuration;
//     
//     public GetByOrganiserAndDateHandler(IUnitOfWork unitOfWork, IUserAccessor userAccessor, IConfiguration configuration)
//     {
//         _unitOfWork = unitOfWork;
//         _userAccessor = userAccessor;
//         _configuration = configuration;
//     }
//     
//     public async Task<GetByOrganiserAndDateResponse> Handle(GetByOrganiserAndDateQuery request, CancellationToken cancellationToken)
//     {
//         var onlyDates = request.Dates.Select(x => x.Date.Date).ToList();
//         var appointments = await _unitOfWork.Appointment.GetAllByOrganiserIdAndDates(request.OrganiserId, onlyDates);
//         var timeZone = _configuration["TimeZone"];
//         foreach (var appointment in appointments)
//         {
//             appointment.StartTime = appointment.StartTime.ConvertToTimeZone(timeZone);
//             appointment.EndTime = appointment.EndTime.ConvertToTimeZone(timeZone);
//         }
//
//         var appointmentsDto = appointments.Adapt<List<GetAppointmentInfoDto>>();
//         return new GetByOrganiserAndDateResponse()
//         {
//             Appointments = appointmentsDto
//         };
//     }
// }