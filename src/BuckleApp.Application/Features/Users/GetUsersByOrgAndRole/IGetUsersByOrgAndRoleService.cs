namespace BuckleApp.Application.Features.Users.GetUsersByOrgAndRole;

public interface IGetUsersByOrgAndRoleService
{
    Task<List<GetUsersByOrgAndRoleResponse>> ExecuteAsync(GetUsersByOrgAndRoleRequest request);
}