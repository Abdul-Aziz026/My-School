using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.TeacherManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.Queries.GetClassTeachers;

public class GetClassTeachersQueryHandler : IRequestHandler<GetClassTeachersQuery, List<TeacherResponseDto>>
{
    private readonly IClassRepository _classRepository;

    public GetClassTeachersQueryHandler(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }

    public async Task<List<TeacherResponseDto>> Handle(GetClassTeachersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var teachers = await _classRepository.GetAllAsync<Teacher>();
            return teachers.Select(t => t.ToTeacherResponseDto()).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occur: {ex.Message}");
        }
    }
}