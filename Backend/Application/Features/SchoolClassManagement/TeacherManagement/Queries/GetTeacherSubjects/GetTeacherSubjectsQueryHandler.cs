using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.SubjectManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.TeacherManagement.Queries.GetTeacherSubjects;

public class GetTeacherSubjectsQueryHandler : IRequestHandler<GetTeacherSubjectsQuery, List<SubjectResponseDto>>
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly ISubjectRepository _subjectRepository;
    public GetTeacherSubjectsQueryHandler(ITeacherRepository teacherRepository, ISubjectRepository subjectRepository)
    {
        _teacherRepository = teacherRepository;
        _subjectRepository = subjectRepository;
    }

    public async Task<List<SubjectResponseDto>> Handle(GetTeacherSubjectsQuery request, CancellationToken cancellationToken)
    {
        var teacher = await _teacherRepository.GetByIdAsync<Teacher>(request.TeacherId);
        if (teacher == null)
        {
            throw new ArgumentException("Teacher not found");
        }
        var subjectResponseDtos = new List<SubjectResponseDto>();
        foreach (var subjectId in teacher.SubjectIds)
        {
            var subject = await _subjectRepository.GetByIdAsync<Subject>(subjectId);
            if (subject is not null)
            {
                subjectResponseDtos.Add(subject.ToSubjectResponseDto());
            }
        }
        return subjectResponseDtos;
    }
}
