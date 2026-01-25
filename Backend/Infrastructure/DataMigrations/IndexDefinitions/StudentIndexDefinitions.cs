
using MongoDB.Bson;
using MongoDB.Driver;
using Infrastructure.DataMigrations.Models;

namespace Infrastructure.DataMigrations.IndexDefinitions.Base;

public class StudentIndexDefinitions : BaseIndexDefinitionProvider
{
    public override IEnumerable<IndexDefinition> GetIndexDefinitions()
    {
        return new List<IndexDefinition>() {
            new IndexDefinition()
            {
                Version = "1.0.0",
                CollectionName = "student",
                IndexName = "idx_student_email",
                Action = IndexAction.Create,
                IndexModel = new CreateIndexModel<BsonDocument>(
                    Builders<BsonDocument>.IndexKeys.Ascending("Email"),
                    new CreateIndexOptions {
                        Name = "idx_student_email",
                        Unique = true
                    }
                )
            },
            // Version 1.0.0 - Add compound index
            new IndexDefinition
            {
                Version = "1.0.0",
                CollectionName = "student",
                IndexName = "idx_student_SchoolId_StudentNumber",
                Action = IndexAction.Create,
                IndexModel = new CreateIndexModel<BsonDocument>(
                    Builders<BsonDocument>.IndexKeys
                            .Ascending("SchoolId")
                            .Ascending("StudentNumber"),
                            new CreateIndexOptions {
                                Name = "idx_student_SchoolId_StudentNumber"
                            }
                )
            }
        };
    }
}
