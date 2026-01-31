
using Application.Common.Interfaces.Services;

namespace Infrastructure.Services;

public class CalendarService : ICalendarService
{
    public async Task<bool> ValidSchoolDay(DateTime date)
    {
        if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday)
        {
            return false;
        }
        await Task.Delay(10); // Simulate async work
        return true;
    }
}


