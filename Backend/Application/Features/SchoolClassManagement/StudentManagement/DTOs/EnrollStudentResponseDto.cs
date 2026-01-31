namespace Application.Features.SchoolClassManagement.StudentManagement.DTOs;

public class EnrollStudentResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string EnrollmentId { get; set; }
    public string PaymentId { get; set; }
}
