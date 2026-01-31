using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.ClassManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.ClassManagement.Queries.GetAllClasses;

public class GetAllClassesQueryHandler : IRequestHandler<GetAllClassesQuery, List<ClassResponseDto>>
{
    private readonly IClassRepository _classRepository;

    public GetAllClassesQueryHandler(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }

    public async Task<List<ClassResponseDto>> Handle(GetAllClassesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _classRepository.GetAllAsync<Class>();
            return result.Select(c => new ClassResponseDto
            {
                Id = c.Id,
                SchoolId = c.SchoolId,
                Name = c.Name,
                Grade = c.Grade,
                Section = c.Section,
                AcademicYear = c.AcademicYear,
                Capacity = c.Capacity,
                Subjects = c.Subjects,
                TeacherIds = c.TeacherIds
            }).ToList();
        }
        catch (Exception ex)
        {
            // Log the exception (logging mechanism not shown here)
            throw new Exception("An error occurred while retrieving classes.", ex);
        }
    }
}