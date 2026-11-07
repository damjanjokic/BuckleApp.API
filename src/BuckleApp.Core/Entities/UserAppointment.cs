namespace BuckleApp.Core.Entities;

public class UserAppointment
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid AppointmentId { get; set; }
    public Appointment Appointment { get; set; }
    public Guid? UserCanceledId { get; set; }
    public User UserCanceled { get; set; }
    public string CancelationReason { get; set; }
}