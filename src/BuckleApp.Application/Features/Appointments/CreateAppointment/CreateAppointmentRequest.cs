using BuckleApp.Application.Validations;
using BuckleApp.Core.Enumerations;

namespace BuckleApp.Application.Features.Appointments.CreateAppointment;

public sealed record CreateAppointmentRequest
{
    [DateTimeUtc]
    [FutureDate(ErrorMessage = "StartTime must be greater than the current time.")]
    public DateTime StartTime { get; set; }
    
    [DateTimeUtc]
    [FutureDate(ErrorMessage = "EndTime must be greater than the current time.")]
    [StartTimeBeforeEndTime("StartTime", ErrorMessage = "EndTime must be greater than StartTime.")]
    public DateTime EndTime { get; set; }

    public string? Note { get; set; }
    public AppointmentType Type { get; set; }
    public Guid? AttendeeId { get; set; }
}