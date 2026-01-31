
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;
using System.Linq.Expressions;
using Application.Common.Extensions;
using Domain.Entities.JunctionEntities;
using Application.Features.SchoolClassManagement.ClassManagement.DTOs;

namespace Application.Features.SchoolClassManagement.StudentManagement.Queries.GetStudentClasses;

public class GetStudentClassesQueryHandler : IRequestHandler<GetStudentClassesQuery, List<ClassResponseDto>>
{
    private readonly IClassRepository _classRepository;
    public GetStudentClassesQueryHandler(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }
    public async Task<List<ClassResponseDto>> Handle(GetStudentClassesQuery request, CancellationToken cancellationToken)
    {
        var enrollments = await _classRepository.GetItemsByConditionAsync<Enrollment>(x => x.StudentId == request.StudentId);
        Expression<Func<Class, bool>> condition = x => true;
        foreach (var enrollment in enrollments)
        {
            condition = condition.Or(x => x.Id == enrollment.ClassId);
        }
        var classes = await _classRepository.GetItemsByConditionAsync<Class>(condition);
        return classes?.Select(x => x.ToClassResponseDto())?.ToList()!;
    }
}
