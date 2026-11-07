using BuckleApp.Core.IUnitOfWork;
using Mapster;

namespace BuckleApp.Application.Features.Organisations.GetOrganisationByName;

public class GetOrganisationByNameService : IGetOrganisationByNameService
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOrganisationByNameService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<GetOrganisationByNameResponse>> ExecuteAsync(string name)
    {
        var organisations = await _unitOfWork.Organisation.GetOrganisationByName(name);
        return organisations.Adapt<List<GetOrganisationByNameResponse>>();
    }
}