using BuckleApp.Application.Features.Users.GetUsersByOrgAndRole;

namespace BuckleApp.Application.Features.Users;

public class UserService : IUserService
{
    private readonly IGetUsersByOrgAndRoleService _getUsersByOrgAndRoleService;

    public UserService(IGetUsersByOrgAndRoleService getUsersByOrgAndRoleService)
    {
        _getUsersByOrgAndRoleService = getUsersByOrgAndRoleService;
    }

    public async Task<List<GetUsersByOrgAndRoleResponse>> GetUserByOrgAndRole(GetUsersByOrgAndRoleRequest request)
        => await _getUsersByOrgAndRoleService.ExecuteAsync(request);
}