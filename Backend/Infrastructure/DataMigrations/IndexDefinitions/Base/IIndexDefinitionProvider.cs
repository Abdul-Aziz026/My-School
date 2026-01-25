
namespace Infrastructure.DataMigrations.IndexDefinitions.Base;

public interface IIndexDefinitionProvider
{
    IEnumerable<IndexDefinition> GetIndexDefinitions();
}
