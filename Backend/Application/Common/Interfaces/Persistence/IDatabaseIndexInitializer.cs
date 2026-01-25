
namespace Application.Common.Interfaces.Persistence;

public interface IDatabaseIndexInitializer
{
    Task InitializeIndexesAsync();
}
