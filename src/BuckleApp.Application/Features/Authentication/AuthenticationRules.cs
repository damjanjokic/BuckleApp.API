using System.Net;
using System.Security.Cryptography;
using System.Text;
using BuckleApp.Application.Exceptions;
using BuckleApp.Core.Entities;
using BuckleApp.Core.IRepositories;

namespace BuckleApp.Application.Features.Authentication;

internal static class AuthenticationRules
{
    internal static RestException InvalidCredentials() => new(HttpStatusCode.Unauthorized, "Invalid credentials");

    internal static async Task<bool> IsEligibleSessionUserAsync(User? user, IAuthRepository auth)
    {
        if (user == null || user.isDeleted || user.IsDisabled || user.LockoutEnd > DateTimeOffset.UtcNow)
            return false;
        var roles = (await auth.GetUserRoles(user)).ToList();
        var supported = new[] { "SuperAdmin", "Admin", "Instructor", "Student" };
        if (roles.Count == 0 || roles.Any(role => !supported.Contains(role)) ||
            (roles.Contains("Student") && roles.Count != 1)) return false;
        return await auth.OrganisationExistsAsync(user.OrganisationId);
    }

    internal static async Task<bool> IsEligibleStudentAsync(User? user, IAuthRepository auth) =>
        await IsEligibleSessionUserAsync(user, auth) &&
        (await auth.GetUserRoles(user!)).SequenceEqual(new[] { "Student" });

    internal static string HashRefreshToken(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
