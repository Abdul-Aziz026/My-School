
using Application.Features.SchoolClassManagement.DTOs;
using Domain.Entities;
using Domain.Entities.JunctionEntities;
using MediatR;

namespace Application.Features.SchoolClassManagement.Commands.EnrollStudent;

/// <summary>
/// return Enrollment Id
/// </summary>
/// <param name="StudentId"></param>
/// <param name="ClassId"></param>
public class EnrollStudentCommand : IRequest<EnrollStudentResponseDto>
{
    public string StudentId { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public string AcademicYear { get; set; }
    public decimal TuitionFee { get; set; }
    public decimal InitialPayment { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public EnrollMentStatus Status { get; set; }
}