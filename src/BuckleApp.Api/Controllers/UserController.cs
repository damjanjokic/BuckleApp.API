using BuckleApp.Application.Features.Users;
using BuckleApp.Application.Features.Users.GetUsersByOrgAndRole;
using BuckleApp.Core.Enumerations;
using Microsoft.AspNetCore.Mvc;

namespace BuckleApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{organisationId}/{role}")]
    public async Task<IActionResult> GetByOrganisationAndRole(Guid organisationId, Role role,[FromQuery] Guid instructorId)
    {
        var request = new GetUsersByOrgAndRoleRequest()
        {
            OrganisationId = organisationId,
            Role = role,
            InstructorId = instructorId
        };

        var response = await _userService.GetUserByOrgAndRole(request);
        return Ok(response);
    }
}