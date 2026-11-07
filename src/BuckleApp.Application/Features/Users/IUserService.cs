using BuckleApp.Application.Features.Users.GetUsersByOrgAndRole;

namespace BuckleApp.Application.Features.Users;

public interface IUserService
{
    Task<List<GetUsersByOrgAndRoleResponse>> GetUserByOrgAndRole(GetUsersByOrgAndRoleRequest request);
}