using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.Queries.GetSubjectClasses;

public class GetSubjectClassesQueryHandler : IRequestHandler<GetSubjectClassesQuery, List<ClassResponseDto>>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IClassRepository _classRepository;

    public GetSubjectClassesQueryHandler(ISubjectRepository subjectRepository, IClassRepository classRepository)
    {
        _subjectRepository = subjectRepository;
        _classRepository = classRepository;
    }
    public async Task<List<ClassResponseDto>> Handle(GetSubjectClassesQuery request, CancellationToken cancellationToken)
    {
        // return all the classes that have the subject id in their subjects list
        var subject = await _subjectRepository.GetByIdAsync<Subject>(request.SubjectId);
        var classes = new List<ClassResponseDto>();
        foreach (var classId in subject.ClassIds)
        {
            var subjectClass = await _classRepository.GetByIdAsync<Class>(classId);
            if (subjectClass is not null)
            {
                classes.Add(subjectClass.ToClassResponseDto());
            }
        }

        return classes;
    }
}