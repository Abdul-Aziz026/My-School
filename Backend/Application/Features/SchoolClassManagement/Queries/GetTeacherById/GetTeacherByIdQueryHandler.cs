using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.Queries.GetTeacherById;

public class GetTeacherByIdQueryHandler : IRequestHandler<GetTeacherByIdQuery, TeacherResponseDto>
{
    private readonly ITeacherRepository _teacherRepository;

    public GetTeacherByIdQueryHandler(ITeacherRepository teacherRepository)
    {
        _teacherRepository = teacherRepository;
    }

    public async Task<TeacherResponseDto> Handle(GetTeacherByIdQuery request, CancellationToken cancellationToken)
    {
        var teacher = await _teacherRepository.GetByIdAsync<Teacher>(request.Id);
        if (teacher is null)
        {
            throw new NotFoundException("Teacher not found");
        }
        return teacher.ToTeacherResponseDto();
    }
}