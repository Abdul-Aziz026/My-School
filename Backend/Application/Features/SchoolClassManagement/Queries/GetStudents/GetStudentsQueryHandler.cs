using System.Linq.Expressions;
using Application.Common.Extensions;
using Application.Common.Interfaces.Repositories;
using Application.Features.Common.Models;
using Application.Features.SchoolClassManagement.DTOs;
using Application.Features.Users.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.Queries.GetStudents;

public class GetStudentsQueryHandler : IRequestHandler<GetStudentsQuery, PagedResult<StudentResponseDto>>
{
    private readonly IStudentRepository _studentRepository;

    public GetStudentsQueryHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<PagedResult<StudentResponseDto>> Handle(GetStudentsQuery query, CancellationToken cancellationToken)
    {
        Expression<Func<Student, bool>> filter = x => true;
        if (!string.IsNullOrWhiteSpace(query.Search))
            filter = filter.And(x => x.Name.Contains(query.Search));

        if (query.Grade.HasValue)
            filter = filter.And(x => x.Grade == query.Grade);

        if (!string.IsNullOrWhiteSpace(query.ClassId))
            filter = filter.And(x => x.ClassId == query.ClassId);
        if (query.Status.HasValue)
        {
            filter = filter.And(x => x.Status.ToString() == query.Status.ToString());
        }

        var totalCount = await _studentRepository.CountAsync<Student>(filter);
        var students = await _studentRepository.GetPagedAsync(filter: filter,
            pageNumber: query.Page,
            pageSize: query.PageSize,
            orderBy:null,
            ascending: query.IsAscending);

        var studentDtos = students.Select(x => x.ToStudentResponseDto()).ToList();

        return new PagedResult<StudentResponseDto>
        {
            Items = studentDtos,
            Total = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }
}