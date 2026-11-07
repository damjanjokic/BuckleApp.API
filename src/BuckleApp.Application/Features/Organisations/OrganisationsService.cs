using BuckleApp.Application.Features.Organisations.CreateOrganisation;
using BuckleApp.Application.Features.Organisations.GetOrganisationByName;

namespace BuckleApp.Application.Features.Organisations;

public class OrganisationsService : IOrganisationsService
{
    private readonly ICreateOrganisationService _createOrganisationService;
    private readonly IGetOrganisationByNameService _getOrganisationByNameService;

    public OrganisationsService(ICreateOrganisationService createOrganisationService, IGetOrganisationByNameService getOrganisationByNameService)
    {
        _createOrganisationService = createOrganisationService;
        _getOrganisationByNameService = getOrganisationByNameService;
    }

    public async Task<Guid> CreateOrganisation(CreateOrganisationRequest request)
        => await _createOrganisationService.ExecuteAsync(request);

    public async Task<List<GetOrganisationByNameResponse>> GetOrganisationsByName(string name)
        => await _getOrganisationByNameService.ExecuteAsync(name);
}