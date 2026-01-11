
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.DTOs;
using Domain.Entities;
using MediatR;
using System.Linq.Expressions;

namespace Application.Features.SchoolClassManagement.Queries.GetTeacherClasses;

public class GetTeacherClassesQueryHandler : IRequestHandler<GetTeacherClassesQuery, List<ClassResponseDto>>
{
    private readonly IClassRepository _classRepository;
    public GetTeacherClassesQueryHandler(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }
    public async Task<List<ClassResponseDto>> Handle(GetTeacherClassesQuery request, CancellationToken cancellationToken)
    {
        var teacherId = request.TeacherId;
        var teacher = await _classRepository.GetByIdAsync<Teacher>(teacherId);
        Expression<Func<Class, bool>> condition = x => x.Id == teacherId;
        var classes = await _classRepository.GetItemsByConditionAsync<Class>(condition);
        return classes?.Select(o => o.ToClassResponseDto())?.ToList()!;
    }
}
