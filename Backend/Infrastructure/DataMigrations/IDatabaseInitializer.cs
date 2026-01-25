
namespace Infrastructure.DataMigrations;

public interface IDatabaseInitializer
{
    Task InitializeAsync();
}
