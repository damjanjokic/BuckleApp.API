using BuckleApp.Application.Features.Organisations;
using Microsoft.AspNetCore.Mvc;

namespace BuckleApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrganisationController : ControllerBase
{
    private readonly IOrganisationsService _organisationsService;

    public OrganisationController(IOrganisationsService organisationsService)
    {
        _organisationsService = organisationsService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchOrganisationByName([FromQuery] string name)
    {
        var response = await _organisationsService.GetOrganisationsByName(name);
        return Ok(response);
    }
}

