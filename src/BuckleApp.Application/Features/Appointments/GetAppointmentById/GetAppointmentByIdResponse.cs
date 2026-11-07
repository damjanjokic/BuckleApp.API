namespace BuckleApp.Application.Features.Appointments.GetAppointmentById;

public class GetAppointmentByIdResponse
{
    public Guid Id { get; set; }
    public string Date { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string Note { get; set; }
    public string Attendee { get; set; }
}