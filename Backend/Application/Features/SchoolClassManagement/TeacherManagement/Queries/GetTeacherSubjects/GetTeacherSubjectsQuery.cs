using Application.Features.SchoolClassManagement.SubjectManagement.DTOs;
using MediatR;
using System;

namespace Application.Features.SchoolClassManagement.TeacherManagement.Queries.GetTeacherSubjects;

public class GetTeacherSubjectsQuery : IRequest<List<SubjectResponseDto>>
{
    public string TeacherId { get; set; }
    public GetTeacherSubjectsQuery(string id)
    {
        TeacherId = id;
    }
}
