using System.ComponentModel.DataAnnotations;
using BuckleApp.Core.Enumerations;

namespace BuckleApp.Application.Features.Authentication.RegisterUser;

public sealed record RegisterUserRequest
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    
    [Required] public string PhoneNumber { get; set; }

    [Required] public string FirstName { get; set; }

    [Required] public string LastName { get; set; }
    
    [Required] public DateTime Birthday { get; set; }
    [Required]
    public Guid OrganisationId { get; set; }

    public Role Role { get; set; }
}