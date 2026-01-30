
using Application.Common.Interfaces.Publisher;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using DocumentFormat.OpenXml.Office2013.PowerPoint;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Infrastructure.Jobs;

public class AbsentStudentNotificationJob : IBackgroundJob
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<AbsentStudentNotificationJob> _logger;
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly ICalendarService _calendarService;
    private readonly IUserRepository _userRepository;

    public string JobName { get; set; } = "AbsentStudentNotificationJob";

    public AbsentStudentNotificationJob(
        IMessageBus messageBus,
        ILogger<AbsentStudentNotificationJob> logger,
        IAttendanceRepository attendanceRepository,
        ICalendarService calendarService,
        IUserRepository userRepository)
    {
        _messageBus = messageBus;
        _logger = logger;
        _attendanceRepository = attendanceRepository;
        _calendarService = calendarService;
        _userRepository = userRepository;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;
        var today = DateTime.Today;
        try
        {
            _logger.LogInformation($"Starting {JobName} at {startTime}");
            if (!await ValidSchoolDay(today))
            {
                var skipMessage = $"Skipping job execution - {today:yyyy-MM-dd} is not a school day";
                _logger.LogInformation(skipMessage);
                return;
            }

            // fetch absent students
            var absentStudents = await GetAbsentStudentsAsync(today);

            // fetch parents
            var parents = await GetAbsentStudentParentsAsync(absentStudents);

            // send notifications
            await SendEmailNotification(today, parents);

            var endTime = DateTime.UtcNow;
            var duration = (endTime - startTime).TotalSeconds;
            _logger.LogInformation($"Completed {JobName} at {endTime}. Duration: {duration} seconds");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while executing {JobName}: {ex.Message}");
            throw;
        }
    }

    private async Task SendEmailNotification(DateTime today, List<User> parents)
    {
        // implement email notification logic here
        throw new NotImplementedException();
    }

    private async Task<List<User>> GetAbsentStudentParentsAsync(List<Student> absentStudents)
    {
        Expression<Func<User, bool>> filter = u => absentStudents
            .Select(s => s.ParentId)
            .Contains(u.Id);
        var parents = await _userRepository.GetItemsByConditionAsync<User>(filter);
        return parents!;
    }

    private async Task<List<Student>> GetAbsentStudentsAsync(DateTime today)
    {
        Expression<Func<Attendance, bool>> filter = x =>
                                            x.Date.Date == today &&
                                            x.IsPresent == false;
        var absentAttendances = await _attendanceRepository.GetItemsByConditionAsync<Attendance>(filter);
        var absentStudentIds = absentAttendances!.Select(a => a.StudentId).Distinct().ToList();
        
        Expression<Func<Student, bool>> studentFilter = s => absentStudentIds.Contains(s.Id);
        var absentStudents = await _userRepository.GetItemsByConditionAsync<Student>(studentFilter);
        return absentStudents!;
    }

    private async Task<bool> ValidSchoolDay(DateTime today)
    {
        return await _calendarService.ValidSchoolDay(today);
    }
}
