
using Application.Common.Interfaces.Publisher;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Contracts.Events;
using DocumentFormat.OpenXml.Office2013.PowerPoint;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Entities;
using Infrastructure.Consumers;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Linq.Expressions;

namespace Infrastructure.Jobs;

public class AbsentStudentNotificationJob : IJob
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

    public async Task Execute(IJobExecutionContext context)
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

            // send notifications to parents
            await NotifyParentAboutAbsence(today, parents);

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

    private async Task NotifyParentAboutAbsence(DateTime today, List<User> parent)
    {
        await SendEmailNotification(today, parent);
        await CreateInAppNotificationAsync(today, parent);
    }

    private async Task SendEmailNotification(DateTime today, List<User> parents)
    {
        foreach (var user in parents)
        {
            var body = EmailNotificationBody(today, user);
            var emailSendCommand = new SendEmailCommand()
            {
                ToMail = user.Email,
                Name = user.UserName,
                Subject = "Student Absence Notification",
                Body = body,
            };
            await _messageBus.PublishAsync(emailSendCommand);
        }
    }

    private async Task CreateInAppNotificationAsync(DateTime today, List<User> parent)
    {
        // TODO: Implement in-app notifications later
        _logger.LogWarning("In-app notifications are not yet implemented.");
        await Task.CompletedTask;
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
    private static string EmailNotificationBody(DateTime today, User user)
    {
        // Using a refined date format for better readability
        string formattedDate = today.ToString("dddd, MMMM dd, yyyy");

        return $@"
    <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; border: 1px solid #eee; border-radius: 8px; overflow: hidden;'>
        <div style='background-color: #1a73e8; padding: 20px; text-align: center;'>
            <h2 style='color: #ffffff; margin: 0;'>Attendance Notification</h2>
        </div>
        
        <div style='padding: 30px;'>
            <p style='font-size: 16px;'>Dear <strong>{user.UserName}</strong>,</p>
            
            <p>We are writing to let you know that your child was marked <strong>absent</strong> from school today:</p>
            
            <div style='background-color: #f8f9fa; border-left: 4px solid #1a73e8; padding: 15px; margin: 20px 0;'>
                <span style='font-size: 18px; font-weight: bold; color: #1a73e8;'>{formattedDate}</span>
            </div>
            
            <p>If you have already notified the school regarding this absence, please disregard this message. If not, please provide an update for our records.</p>
            
            <div style='margin-top: 30px;'>
                <a href='mailto:azizulcsebsmrstu@gmail.com' style='background-color: #1a73e8; color: white; padding: 12px 25px; text-decoration: none; border-radius: 4px; font-weight: bold; display: inline-block;'>Contact Administration</a>
            </div>
        </div>
        
        <div style='background-color: #f1f3f4; padding: 20px; text-align: center; font-size: 12px; color: #666;'>
            <p>This is an automated message from <strong>[School Name]</strong>.<br>
            If you believe this record is in error, please call our office immediately.</p>
        </div>
    </div>";
    }
}
