using Application.Features.Common.Models;
using Application.Features.SchoolClassManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.Queries.GetStudents;

public class GetStudentsQuery : IRequest<PagedResult<StudentResponseDto>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Search { get; set; }
    public int? Grade { get; set; }
    public string? ClassId { get; set; }
    public StudentStatus? Status { get; set; }
    public bool IsAscending { get; set; } = true;
}

