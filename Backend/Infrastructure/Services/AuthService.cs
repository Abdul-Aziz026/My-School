
using Application.Interfaces;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{

    public Task<string> LoginAsync(string email, string password)
    {
        throw new NotImplementedException();
    }

    public Task<string> RegisterAsync(string email, string password)
    {
        throw new NotImplementedException();
    }
}
