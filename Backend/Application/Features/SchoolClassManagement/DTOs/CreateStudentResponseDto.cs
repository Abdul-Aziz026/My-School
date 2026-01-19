namespace Application.Features.SchoolClassManagement.DTOs;

public class CreateStudentResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string StudentId { get; set; }
    public string StudentNumber { get; set; }
}
