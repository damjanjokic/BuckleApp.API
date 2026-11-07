using System.Net;
using BuckleApp.Application.Exceptions;
using BuckleApp.Core.Enumerations;
using BuckleApp.Core.IRepositories;
using BuckleApp.Core.IUnitOfWork;
using BuckleApp.Infrastructure.Interfaces;
using Mapster;

namespace BuckleApp.Application.Features.Users.GetUsersByOrgAndRole;

public class GetUsersByOrgAndRoleService : IGetUsersByOrgAndRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserAccessor _userAccessor;
    private readonly IAuthRepository _authRepository;
    
    public async Task<List<GetUsersByOrgAndRoleResponse>> ExecuteAsync(GetUsersByOrgAndRoleRequest request)
    {
        var currentUser = await _unitOfWork.User.GetById(_userAccessor.GetCurrentUserId());
        var currentRoles = await _authRepository.GetUserRoles(currentUser);
        if (currentRoles.First() == Role.Instructor.ToString() && request.InstructorId != Guid.Empty)
            throw new RestException(HttpStatusCode.Unauthorized, "You cannot see students of other instructors");
        
        var users = 
            await _unitOfWork.User.GetByOrganisationAndRole(request.OrganisationId, request.Role, request.InstructorId);

        return users.Adapt<List<GetUsersByOrgAndRoleResponse>>();
    }
}