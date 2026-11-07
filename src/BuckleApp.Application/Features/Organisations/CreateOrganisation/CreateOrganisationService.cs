using System.Net;
using BuckleApp.Application.Exceptions;
using BuckleApp.Core.Entities;
using BuckleApp.Core.IUnitOfWork;
using BuckleApp.Infrastructure.Interfaces;
using Mapster;

namespace BuckleApp.Application.Features.Organisations.CreateOrganisation;

public class CreateOrganisationService : ICreateOrganisationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    
    public async Task<Guid> ExecuteAsync(CreateOrganisationRequest request)
    {
        var organisation = request.Adapt<Organisation>();
        var organisationExists = await _unitOfWork.Organisation.OrganisationExists(organisation);

        if (organisationExists)
        {
            throw new RestException(HttpStatusCode.BadRequest,
                "Organisation under that name and address already exist");
        }

        await _unitOfWork.Organisation.CreateOrganisation(organisation);
        await _unitOfWork.SaveAsync();

        return organisation.Id;
    }
}