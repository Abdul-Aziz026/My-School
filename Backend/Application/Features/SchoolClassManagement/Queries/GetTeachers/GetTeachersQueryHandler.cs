
using Application.Common.Extensions;
using Application.Common.Interfaces.Repositories;
using Application.Features.Common.Models;
using Application.Features.SchoolClassManagement.DTOs;
using Domain.Entities;
using MediatR;
using System.Collections;
using System.Linq.Expressions;

namespace Application.Features.SchoolClassManagement.Queries.GetTeachers;

public class GetTeachersQueryHandler : IRequestHandler<GetTeachersQuery, PagedResult<TeacherResponseDto>>
{
    private readonly ITeacherRepository _teacherRepository;
    public GetTeachersQueryHandler(ITeacherRepository teacherRepository)
    {
        _teacherRepository = teacherRepository;
    }
    public async Task<PagedResult<TeacherResponseDto>> Handle(GetTeachersQuery query, CancellationToken cancellationToken)
    {
        Expression<Func<Teacher, bool>>? filter = x => true;
        if (!string.IsNullOrWhiteSpace(query.Search))
            filter = filter.And(x => x.Name.Contains(query.Search));

        filter = filter.And(x => x.Status == query.Status);

        var totalCount = await _teacherRepository.CountAsync<Teacher>(filter);

        var teachers = await _teacherRepository.GetPagedAsync(filter: filter,
            pageNumber: query.Page,
            pageSize: query.PageSize,
            orderBy: null,
            ascending: true);

        var teacherDtos = teachers.Select(x => x.ToTeacherResponseDto()).ToList();

        return new PagedResult<TeacherResponseDto>
        {
            Items = teacherDtos,
            Total = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }
}
