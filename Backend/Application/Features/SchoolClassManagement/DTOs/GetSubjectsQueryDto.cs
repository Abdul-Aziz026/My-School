using Application.Features.SchoolClassManagement.Queries.GetSubjects;

namespace Application.Features.SchoolClassManagement.DTOs;

public class GetSubjectsQueryDto
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string? Search { get; set; }
    public bool? IsActive { get; set; }

    public GetSubjectsQuery ToGetSubjectsQuery()
    {
        return new GetSubjectsQuery()
        {
            Page = this.Page ?? 1,
            PageSize = this.PageSize ?? 10,
            Search = this.Search,
            IsActive = this.IsActive
        };
    }
}