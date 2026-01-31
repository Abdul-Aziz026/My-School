
using Application.Features.Common.Models;
using Application.Features.SchoolClassManagement.TeacherManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.Queries.GetTeachers;

public class GetTeachersQuery : IRequest<PagedResult<TeacherResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public TeacherStatus Status { get; set; }
    public string? Department { get; set; }
    public string? Search { get; set; }
}
