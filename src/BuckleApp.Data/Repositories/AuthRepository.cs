using System.Linq.Expressions;
using System.Security.Claims;
using BuckleApp.Core.Entities;
using BuckleApp.Core.Enumerations;
using BuckleApp.Core.IRepositories;
using BuckleApp.Data.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BuckleApp.Data.Repositories;

public class AuthRepository : IAuthRepository
{
        private readonly UserManager<User> _userManager;
        private readonly ApplicationContext _context;

        public AuthRepository(UserManager<User> userManager, ApplicationContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IdentityResult> CreateWithRoleAsync(User user, string? password, Role role)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            var created = password == null ? await CreateAsync(user) : await CreateAsync(user, password);
            if (!created.Succeeded) return created;
            var assigned = await _userManager.AddToRoleAsync(user, role.ToString());
            if (!assigned.Succeeded) return assigned;
            await transaction.CommitAsync();
            return assigned;
        }

        public Task<IdentityResult> CreateAsync(User user) => _userManager.CreateAsync(user);
        public async Task<User?> FindByIdAsync(Guid id) => await _userManager.FindByIdAsync(id.ToString());
        public Task<bool> OrganisationExistsAsync(Guid id) => _context.Organisations.AnyAsync(x => x.Id == id && !x.IsDeleted);
        public Task<List<UserLoginCode>> GetLoginCodesAsync(Guid userId) => _context.UserLoginCodes.Where(x => x.UserId == userId).ToListAsync();
        public Task<RefreshToken?> FindRefreshTokenAsync(string hash) => _context.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == hash);
        public Task<List<RefreshToken>> GetRefreshFamilyAsync(Guid userId, Guid familyId) => _context.RefreshTokens.Where(x => x.UserId == userId && x.FamilyId == familyId).ToListAsync();
        public void AddLoginCode(UserLoginCode code) => _context.UserLoginCodes.Add(code);
        public void AddRefreshToken(RefreshToken token) => _context.RefreshTokens.Add(token);

        // Every code/session mutation locks the owning user, including across API instances.
        public async Task<T> WithUserLockAsync<T>(Guid userId, Func<Task<T>> operation)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            await _context.Database.ExecuteSqlInterpolatedAsync($"SELECT 1 FROM \"AspNetUsers\" WHERE \"Id\" = {userId} FOR UPDATE");
            // A refresh token may have been read before waiting for the lock.
            _context.ChangeTracker.Clear();
            var result = await operation();
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return result;
        }

        public async Task<bool> CheckPasswordSignInAsync(User user, string password, bool lockoutOnFailure)
        {
            if (await _userManager.IsLockedOutAsync(user)) return false;
            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                if (lockoutOnFailure) await _userManager.AccessFailedAsync(user);
                return false;
            }
            await _userManager.ResetAccessFailedCountAsync(user);
            return true;
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string decodedToken)
        {
            return await _userManager.ConfirmEmailAsync(user, decodedToken);
        }

        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<User> FindByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public Task UpdateAsync(User user)
        {
            return _userManager.UpdateAsync(user);
        }

        public async Task<bool> UserExistsQueryAsync(Expression<Func<User, bool>> filter = null)
        {
            if (await _context.Users.AnyAsync(filter))
                return true;

            return false;
        }

        public async Task<bool> AddToRoleAsync(User user, Role role)
        {
            var result = await _userManager.AddToRoleAsync(user, role.ToString());
            return result.Succeeded;
        }

        public async Task<IEnumerable<string>> GetUserRoles(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync(User user)
        {
            return await _userManager.GetClaimsAsync(user);
        }

        public async Task<IdentityResult> AddClaimAsync(User user, Claim claim)
        {
            return await _userManager.AddClaimAsync(user, claim);
            
        }

        public async Task<IdentityResult> RemoveClaimAsync(User user, Claim claim)
        {
           return await _userManager.RemoveClaimAsync(user, claim);
        }
}