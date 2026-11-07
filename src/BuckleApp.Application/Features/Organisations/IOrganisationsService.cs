using BuckleApp.Application.Features.Organisations.CreateOrganisation;
using BuckleApp.Application.Features.Organisations.GetOrganisationByName;

namespace BuckleApp.Application.Features.Organisations;

public interface IOrganisationsService
{
    Task<Guid> CreateOrganisation(CreateOrganisationRequest request);
    Task<List<GetOrganisationByNameResponse>> GetOrganisationsByName(string name);
}