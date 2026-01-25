using MongoDB.Driver;

namespace Infrastructure.DataMigrations.Indexing.Indexes;

public class StudentEmailIndex : IndexDefinitionBase
{
    public override string CollectionName => "student";
    public override IndexKeysDefinition<dynamic> Keys => 
        Builders<dynamic>.IndexKeys
                .Ascending("email")
                .Descending("createdAt");

    public override CreateIndexOptions Options => 
        new CreateIndexOptions
        {
            Name = "IX_Student_Email_CreatedAt",
            Unique = true,
            Background = true
        };

    public override string Description => 
        "Ensures unique email addresses for students, sorted by creation date.";
}
