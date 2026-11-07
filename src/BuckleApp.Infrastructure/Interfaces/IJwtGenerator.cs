using BuckleApp.Core.Enumerations;
using BuckleApp.Core.Entities;

namespace BuckleApp.Infrastructure.Interfaces;

public interface IJwtGenerator
{
    string CreateToken(User user, List<string> roles);
}