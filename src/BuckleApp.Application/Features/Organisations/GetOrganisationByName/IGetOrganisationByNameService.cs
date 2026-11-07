namespace BuckleApp.Application.Features.Organisations.GetOrganisationByName;

public interface IGetOrganisationByNameService
{
    Task<List<GetOrganisationByNameResponse>> ExecuteAsync(string name);
}