using BuckleApp.Core.Entities;
using BuckleApp.Core.IRepositories;
using BuckleApp.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace BuckleApp.Data.Repositories;

public class OrganisationRepository : Repository<Organisation>,IOrganisationRepository
{
    public OrganisationRepository(ApplicationContext context) : base(context)
    {
    }

    public async Task CreateOrganisation(Organisation organisation)
    {
        await AddAsync(organisation);
    }

    public async Task<bool> OrganisationExists(Organisation organisation) =>
        await Context.Organisations
            .AnyAsync(x => x.Name == organisation.Name && x.Address == organisation.Address);

    public async Task<List<Organisation>> GetOrganisationByName(string name) =>
        await Context.Organisations
            .Where(x => x.Name.Contains(name)).ToListAsync();
}