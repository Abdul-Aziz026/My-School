using Application.Features.Common.Models;
using Application.Features.SchoolClassManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.Queries.GetSubjects;

public class GetSubjectsQuery : IRequest<PagedResult<SubjectResponseDto>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsAscending { get; set; } = true;
}