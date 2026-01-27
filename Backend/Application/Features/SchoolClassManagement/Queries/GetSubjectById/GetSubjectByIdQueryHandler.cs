using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.Queries.GetSubjectById;

public class GetSubjectByIdQueryHandler : IRequestHandler<GetSubjectByIdQuery, SubjectResponseDto>
{
    private readonly ISubjectRepository _subjectRepository;

    public GetSubjectByIdQueryHandler(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<SubjectResponseDto> Handle(GetSubjectByIdQuery request, CancellationToken cancellationToken)
    {
        var subject = await _subjectRepository.GetByIdAsync<Subject>(request.Id);
        if (subject is null)
        {
            throw new NotFoundException("Subject not found");
        }
        return subject.ToSubjectResponseDto();
    }
}