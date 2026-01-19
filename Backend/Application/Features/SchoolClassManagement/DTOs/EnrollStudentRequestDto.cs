
using Application.Features.SchoolClassManagement.Commands.EnrollStudent;
using Application.Features.SchoolClassManagement.Commands.UnenrollStudent;
using Domain.Entities.JunctionEntities;

namespace Application.Features.SchoolClassManagement.DTOs;

public class EnrollStudentRequestDto
{
    public string StudentId { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    public decimal TuitionFee { get; set; }
    public EnrollMentStatus Status { get; set; } = EnrollMentStatus.Enrolled;


    public EnrollStudentCommand ToEnrollStudentCommand()
    {
        return new EnrollStudentCommand
        {
            StudentId = StudentId,
            ClassId = ClassId,
            AcademicYear = AcademicYear,
            TuitionFee = TuitionFee,
            EnrollmentDate = EnrollmentDate,
            Status = Status
        };
    }
}
