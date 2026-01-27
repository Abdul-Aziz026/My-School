using Application.Features.SchoolClassManagement.DTOs;
using MediatR;
using System;

namespace Application.Features.SchoolClassManagement.Queries.GetTeacherSubjects;

public class GetTeacherSubjectsQuery : IRequest<List<SubjectResponseDto>>
{
    public string TeacherId { get; set; }
    public GetTeacherSubjectsQuery(string id)
    {
        TeacherId = id;
    }
}
