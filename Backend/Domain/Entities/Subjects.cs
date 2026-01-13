
namespace Domain.Entities;

public class Subjects : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty ;
    public int Credits { get; set; }
    public string Description { get; set; } = string.Empty;
}
