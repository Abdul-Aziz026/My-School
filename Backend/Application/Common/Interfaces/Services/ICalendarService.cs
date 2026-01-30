namespace Application.Common.Interfaces.Services;

public interface ICalendarService
{
    Task<bool> ValidSchoolDay(DateTime date);
}
