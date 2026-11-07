namespace BuckleApp.Application.Dtos;

public class GetAppointmentInfoDto
{
    public Guid Id { get; set; }
    public string Date { get; set; }
    public string Day { get; set; }
    public string Time { get; set; }
    public string Note { get; set; }
    public bool isSet { get; set; }
    public string Attendee { get; set; }
    public bool isCanceled { get; set; }
    public string UserCanceled { get; set; }
    public int Type { get; set; }
}