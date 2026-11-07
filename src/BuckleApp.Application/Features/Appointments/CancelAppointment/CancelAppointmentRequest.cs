using System.ComponentModel.DataAnnotations;

namespace BuckleApp.Application.Features.Appointments.CancelAppointment;

public class CancelAppointmentRequest
{
    [Required]
    public string Reason { get; set; }
}