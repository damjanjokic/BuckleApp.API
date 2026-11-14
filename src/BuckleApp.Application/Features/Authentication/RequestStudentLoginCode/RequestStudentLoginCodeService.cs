using BuckleApp.Infrastructure.Security;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using BuckleApp.Core.Entities;
using BuckleApp.Core.IRepositories;
using BuckleApp.Core.IUnitOfWork;
using BuckleApp.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace BuckleApp.Application.Features.Authentication.RequestStudentLoginCode;

public class RequestStudentLoginCodeService : IRequestStudentLoginCodeService
{
    private readonly AuthenticationOptions _options;
    private readonly IAuthRepository _authRepository;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoginCodeHasher _loginCodeHasher;
    private readonly ILogger<RequestStudentLoginCodeService> _logger;

    public RequestStudentLoginCodeService(IAuthRepository authRepository, IEmailService emailService, IUnitOfWork unitOfWork, ILoginCodeHasher loginCodeHasher, ILogger<RequestStudentLoginCodeService> logger, IOptions<AuthenticationOptions> options)
    {
        _authRepository = authRepository;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _loginCodeHasher = loginCodeHasher;
        _logger = logger;
        _options = options.Value;
    }

    public async Task ExecuteAsync(RequestStudentLoginCodeRequest request)
    {
        var user = await _authRepository.FindByEmailAsync(request.Email.Trim());
        if (user == null) return;
        await _authRepository.WithUserLockAsync(user.Id, async () =>
        {
            var current = await _authRepository.FindByIdAsync(user.Id);
            if (!await AuthenticationRules.IsEligibleStudentAsync(current, _authRepository)) return false;
            var now = DateTime.UtcNow;
            var codes = await _authRepository.GetLoginCodesAsync(user.Id);

            if (codes.Any(x => x.CreatedAt > now.AddMinutes(-1)) ||
                codes.Count(x => x.CreatedAt > now.AddHours(-1)) >= 5) return false;
            foreach (var old in codes.Where(x => x.ConsumedAt == null)) old.ConsumedAt = now;
            var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var entry = new UserLoginCode { Id = Guid.NewGuid(), UserId = user.Id,
                CreatedAt = now, ExpiresAt = now.AddMinutes(_options.LoginCodeLifetimeMinutes) };
            entry.CodeHash = _loginCodeHasher.Hash(user.Id, entry.Id, code);
 
            await _unitOfWork.SaveAsync();
            _authRepository.AddLoginCode(entry);
            try
            {
                if (!await _emailService.Send("BuckleApp login code", $"Your login code is {code}. It expires in {_options.LoginCodeLifetimeMinutes} minutes.", current!.Email!))
                    entry.ConsumedAt = now;
            }
            catch
            {
                entry.ConsumedAt = now;
                _logger.LogWarning("Student login email delivery failed.");
            }
            return true;
        });
    }
}
