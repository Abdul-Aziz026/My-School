
namespace Application.Settings;

public class AuthLockoutSettings
{
    public int MaxFailedLoginAttempts { get; set; }
    public int LockoutDuration { get; set; }
}
