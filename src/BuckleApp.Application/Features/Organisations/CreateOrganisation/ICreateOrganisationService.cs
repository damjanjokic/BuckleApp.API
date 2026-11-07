namespace BuckleApp.Application.Features.Organisations.CreateOrganisation;

public interface ICreateOrganisationService
{
    Task<Guid> ExecuteAsync(CreateOrganisationRequest request);
}