using Domain.Entities;

namespace Application.Features.SchoolClassManagement.SubjectManagement.DTOs;

public class SubjectResponseDto
{
    public string Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int Credits { get; set; }
}

public static class SubjectResponseDtoExtensions
{
    public static SubjectResponseDto ToSubjectResponseDto(this Subject subject)
    {
        return new SubjectResponseDto()
        {
            Id = subject.Id,
            Name = subject.Name,
            Code = subject.Code,
            Credits = subject.Credits
        };
    }
}