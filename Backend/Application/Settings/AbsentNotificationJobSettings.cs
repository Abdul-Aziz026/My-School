using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Settings;

public class AbsentNotificationJobSettings
{
    public string ScheduleTime { get; set; } = "06:00:00"; // Default to 6 AM

    // Helper to convert the string to a Cron expression
    public static string GetCronExpression(string scheduleTime)
    {
        var time = TimeSpan.Parse(scheduleTime);
        // Format: "seconds minutes hours day-of-month month day-of-week"
        return $"0 {time.Minutes} {time.Hours} * * ?";
    }
}
