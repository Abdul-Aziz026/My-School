using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.DTOs;

public class RecordExitDto
{
    public string StudentId { get; set; } = string.Empty;
    public DateTime ExitTime { get; set; }
}
