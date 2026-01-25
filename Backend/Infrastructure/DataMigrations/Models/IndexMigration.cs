
public class IndexMigration
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Version { get; set; }
    public string CollectionName { get; set; }
    public string IndexName { get; set; }
    public DateTime AppliedAt { get; set; }
}
