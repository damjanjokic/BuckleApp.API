using BuckleApp.Core.Entities;
using BuckleApp.Core.Enumerations;
using BuckleApp.Core.IRepositories;
using BuckleApp.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace BuckleApp.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationContext context) : base(context)
    {

    }

    public async Task<User> GetById(Guid Id) => await GetAsync(Id);

    public async Task<List<User>> GetByOrganisationAndRole(Guid organisationId, Role role, Guid? instructorId)
    {
       var query = Context.Users.Join(
                Context.UserRoles,
                user => user.Id,
                userRole => userRole.UserId,
                (user, userRole) => new { user, userRole })
            .Join(
                Context.Roles,
                ur => ur.userRole.RoleId,
                role => role.Id,
                (ur, role) => new { ur.user, role })
            .Where(res => res.user.OrganisationId == organisationId && res.role.Name == role.ToString())
            .AsQueryable();

       if (instructorId != Guid.Empty)
           query = query.Where(x => x.user.InstructorId == instructorId);

       return await query.Select(x => x.user).ToListAsync();
    }
}