using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.Queries.GetAllClasses;

public class GetAllClassesQueryHandler : IRequestHandler<GetAllClassesQuery, List<ClassResponseDto>>
{
    private readonly IClassRepository _classRepository;

    public GetAllClassesQueryHandler(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }

    public Task<List<ClassResponseDto>> Handle(GetAllClassesQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}