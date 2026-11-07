using System.ComponentModel.DataAnnotations;

namespace BuckleApp.Application.Features.Organisations.CreateOrganisation;

public class CreateOrganisationRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    public string Email { get; set; }
}