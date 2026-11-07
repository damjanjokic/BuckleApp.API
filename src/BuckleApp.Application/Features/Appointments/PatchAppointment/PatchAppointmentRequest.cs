namespace BuckleApp.Application.Features.Appointments.PatchAppointment;

public sealed record PatchAppointmentRequest
{
    public Guid Id { get; set; }

    public string Note { get; set; }
    public int Type { get; set; }
}