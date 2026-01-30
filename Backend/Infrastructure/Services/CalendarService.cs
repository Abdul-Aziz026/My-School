
using Application.Common.Interfaces.Services;

namespace Infrastructure.Services;

public class CalendarService : ICalendarService
{
    public async Task<bool> ValidSchoolDay(DateTime date)
    {
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
        {
            return false;
        }
        await Task.Delay(10); // Simulate async work
        return true;
    }
}


