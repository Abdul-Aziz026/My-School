
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.DTOs;
using Domain.Entities;
using MediatR;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace Application.Features.SchoolClassManagement.Queries.GetClassStudents;

public class GetClassStudentsQueryHandler : IRequestHandler<GetClassStudentsQuery, List<StudentResponseDto>>
{
    private readonly IClassRepository _classRepository;
    public GetClassStudentsQueryHandler(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }

    public async Task<List<StudentResponseDto>> Handle(GetClassStudentsQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Class, bool>> condition = x => x.Id == request.ClassId;
        var classResponse = await _classRepository.GetItemByConditionAsync<Class>(condition);

        Expression<Func<Student, bool>> studentFindCondition = x => x.Id == request.ClassId;
        var students = await _classRepository.GetItemsByConditionAsync<Student>(studentFindCondition);
        return students?.Select(x => x.ToStudentResponseDto())?.ToList()!;
    }
}
