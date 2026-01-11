
namespace Domain.Entities;

public class Teacher : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string SchoolId { get; set; } = string.Empty;
    public List<string> ClassIds {  get; set; } = new();
}
