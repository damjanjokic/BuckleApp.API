using BuckleApp.Core.Entities;
using BuckleApp.Core.Enumerations;

namespace BuckleApp.Core.IRepositories;

public interface IUserRepository
{
    public Task<User> GetById(Guid Id);
    public Task<List<User>> GetByOrganisationAndRole(Guid organisationId, Role role, Guid? instructorId);
}