
namespace Domain.Entities;

public class Student : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string SchoolId { get; set; } = string.Empty;
    public int Grade { get; set; }
    public string Section {  get; set; } = string.Empty;
}
