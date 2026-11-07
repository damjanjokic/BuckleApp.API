namespace BuckleApp.Application.Features.Appointments.GetUpcomingForUserByDate;

public class UpcomingAppointmentDto
{
    public string FullName { get; set; }
    public string Location { get; set; }
    public DateTime StartTime { get; set; }
    public string OrganiserPhone { get; set; }
    public string AttendeePhone { get; set; }
}