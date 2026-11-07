using System.ComponentModel.DataAnnotations;
using BuckleApp.Core.Enumerations;

namespace BuckleApp.Application.Features.Users.GetUsersByOrgAndRole;

public class GetUsersByOrgAndRoleRequest
{
    [Required]
    public Guid OrganisationId { get; set; }
    [Required]
    public Role Role { get; set; }
    public Guid InstructorId { get; set; }
    
}