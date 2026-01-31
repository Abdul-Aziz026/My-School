using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.ClassManagement.DTOs;
using Domain.Entities;
using MediatR;
using System.Linq.Expressions;
using static System.Reflection.Metadata.BlobBuilder;

namespace Application.Features.SchoolClassManagement.ClassManagement.Queries.GetClassById;

public class GetClassByIdQueryHandler : IRequestHandler<GetClassByIdQuery, ClassResponseDto>
{
    private readonly IClassRepository _classRepository;
    public GetClassByIdQueryHandler(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }

    public async Task<ClassResponseDto> Handle(GetClassByIdQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
        {
            throw new ArgumentNullException($"Id not round");
        }
        Expression<Func<Class, bool>> condition = x => x.Id == request.Id;
        var classResponse = await _classRepository.GetItemByConditionAsync<Class>(condition);
        if (classResponse == null)
        {
            throw new NotFoundException("Class not found");
        }
        return classResponse.ToClassResponseDto();
    }
}
