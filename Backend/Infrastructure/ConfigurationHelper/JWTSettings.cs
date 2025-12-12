
namespace Infrastructure.ConfigurationHelper;

public class JWTSettings
{
    public string SecretKey { get; set; } = null!;
    public int ExpiryInMinutes { get; set; }
}
