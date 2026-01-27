using Application.Common.Extensions;
using Application.Common.Interfaces.Repositories;
using Application.Features.Common.Models;
using Application.Features.SchoolClassManagement.DTOs;
using Domain.Entities;
using MediatR;
using System.Linq.Expressions;

namespace Application.Features.SchoolClassManagement.Queries.GetSubjects;

public class GetSubjectsQueryHandler : IRequestHandler<GetSubjectsQuery, PagedResult<SubjectResponseDto>>
{
    private readonly ISubjectRepository _subjectRepository;

    public GetSubjectsQueryHandler(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<PagedResult<SubjectResponseDto>> Handle(GetSubjectsQuery query, CancellationToken cancellationToken)
    {
        Expression<Func<Subject, bool>> filter = x => true;
        if (!string.IsNullOrWhiteSpace(query.Search))
            filter = filter.And(x => x.Name.Contains(query.Search));

        if (query.IsActive.HasValue)
            filter = filter.And(x => x.IsActive == query.IsActive);

        var totalCount = await _subjectRepository.CountAsync<Subject>(filter);

        var subjects = await _subjectRepository.GetPagedAsync(filter: filter,
            pageNumber: query.Page,
            pageSize: query.PageSize,
            orderBy: null,
            ascending: query.IsAscending.Value);

        var subjectDtos = subjects.Select(x => x.ToSubjectResponseDto()).ToList();

        return new PagedResult<SubjectResponseDto>
        {
            Items = subjectDtos,
            Total = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }
}