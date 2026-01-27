using Domain.Entities;

namespace Application.Features.SchoolClassManagement.DTOs;

public class GetStudentsQueryDto
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string? Search { get; set; }
    public int? GradeLevel { get; set; }
    public string? ClassId { get; set; }
    public StudentStatus? Status { get; set; }
}
