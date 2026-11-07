using BuckleApp.Core.Entities;

namespace BuckleApp.Core.IRepositories;

public interface IOrganisationRepository : IRepository<Organisation>
{
    Task CreateOrganisation(Organisation organisation);
    Task<bool> OrganisationExists(Organisation organisation);
    Task<List<Organisation>> GetOrganisationByName(string name);
}