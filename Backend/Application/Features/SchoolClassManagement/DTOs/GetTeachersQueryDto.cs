
using Application.Features.SchoolClassManagement.Queries.GetTeachers;
using Domain.Entities;

namespace Application.Features.SchoolClassManagement.DTOs;

public class GetTeachersQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public TeacherStatus? Status { get; set; }
    public string? Department { get; set; }

    /// <summary>
    /// Search term for teacher name or email
    /// </summary>
    public string? Search { get; set; }

    public GetTeachersQuery ToGetTeacherQuery()
    {
        return new GetTeachersQuery()
        {
            Page = this.Page,
            PageSize = this.PageSize,
            Status = this.Status ?? TeacherStatus.Active,
            Department = this.Department,
            Search = this.Search
        };
    }
}
