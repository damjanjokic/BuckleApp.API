namespace BuckleApp.Infrastructure.Interfaces;

public interface IUserAccessor
{
    Guid GetCurrentUserId();
}