using Microsoft.AspNetCore.Identity;

namespace BuckleApp.Core.Entities;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Birthday { get; set; }
    public int AvailableCancellations { get; set; }

    public Guid OrganisationId { get; set; }
    public Organisation Organisation { get; set; }
    
    public Guid? InstructorId { get; set; }
    public User? Instructor { get; set; }
    public ICollection<UserAppointment> UserAppointments { get; set; }
    public bool IsDisabled { get; set; }
    public bool isDeleted { get; set; }
}