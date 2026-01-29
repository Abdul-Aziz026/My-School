namespace Application.Features.SchoolClassManagement.AttendanceManagement.DTOs;

public class AttendanceFileResultDto
{
    public string ContentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
}