using Application.Common.Interfaces.Publisher;
using Application.Features.Dashboard.Queries.GetDashboardStats;
using Application.Features.Dashboard.Queries.GetRecentActivities;
using Application.Features.Dashboard.Queries.GetUpcomingEvents;
using Application.Features.Dashboard.Queries.GetAnnouncements;
using Application.Features.Dashboard.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : Controller
{
    private readonly IMessageBus _messageBus;

    public HomeController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Gets dashboard overview with key statistics
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var query = new GetDashboardStatsQuery();
        var result = await _messageBus.SendAsync<GetDashboardStatsQuery, DashboardStatsDto>(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets recent activities in the system
    /// </summary>
    [HttpGet("recent-activities")]
    public async Task<IActionResult> GetRecentActivities([FromQuery] int pageSize = 10)
    {
        var query = new GetRecentActivitiesQuery(pageSize);
        var result = await _messageBus.SendAsync<GetRecentActivitiesQuery, List<ActivityDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets upcoming events and important dates
    /// </summary>
    [HttpGet("upcoming-events")]
    public async Task<IActionResult> GetUpcomingEvents([FromQuery] int days = 30)
    {
        var query = new GetUpcomingEventsQuery(days);
        var result = await _messageBus.SendAsync<GetUpcomingEventsQuery, List<EventDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets system announcements
    /// </summary>
    [HttpGet("announcements")]
    public async Task<IActionResult> GetAnnouncements([FromQuery] bool activeOnly = true)
    {
        var query = new GetAnnouncementsQuery(activeOnly);
        var result = await _messageBus.SendAsync<GetAnnouncementsQuery, List<AnnouncementDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets complete home page data (dashboard stats, activities, events, announcements)
    /// </summary>
    [HttpGet("overview")]
    public async Task<IActionResult> GetHomePageOverview()
    {
        var statsQuery = new GetDashboardStatsQuery();
        var activitiesQuery = new GetRecentActivitiesQuery(5);
        var eventsQuery = new GetUpcomingEventsQuery(30);
        var announcementsQuery = new GetAnnouncementsQuery(true);

        var stats = await _messageBus.SendAsync<GetDashboardStatsQuery, DashboardStatsDto>(statsQuery);
        var activities = await _messageBus.SendAsync<GetRecentActivitiesQuery, List<ActivityDto>>(activitiesQuery);
        var events = await _messageBus.SendAsync<GetUpcomingEventsQuery, List<EventDto>>(eventsQuery);
        var announcements = await _messageBus.SendAsync<GetAnnouncementsQuery, List<AnnouncementDto>>(announcementsQuery);

        var overview = new HomePageOverviewDto
        {
            DashboardStats = stats,
            RecentActivities = activities,
            UpcomingEvents = events,
            Announcements = announcements
        };

        return Ok(overview);
    }

    /// <summary>
    /// Gets quick stats summary
    /// </summary>
    [HttpGet("quick-stats")]
    public async Task<IActionResult> GetQuickStats()
    {
        var query = new GetDashboardStatsQuery();
        var result = await _messageBus.SendAsync<GetDashboardStatsQuery, DashboardStatsDto>(query);
        
        var quickStats = new
        {
            result.TotalStudents,
            result.TotalTeachers,
            result.TotalClasses,
            result.ActiveSessions
        };

        return Ok(quickStats);
    }
}


// Application/Features/Dashboard/DTOs/DashboardStatsDto.cs
namespace Application.Features.Dashboard.DTOs;

public class DashboardStatsDto
{
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalClasses { get; set; }
    public int ActiveSessions { get; set; }
    public int TotalSubjects { get; set; }
    public int PendingAssignments { get; set; }
    public int UpcomingExams { get; set; }
    public decimal AttendanceRate { get; set; }
    public DateTime LastUpdated { get; set; }
}

// Application/Features/Dashboard/DTOs/ActivityDto.cs
namespace Application.Features.Dashboard.DTOs;

public class ActivityDto
{
    public string Id { get; set; }
    public string ActivityType { get; set; } // e.g., "StudentEnrolled", "ClassCreated", "GradePosted"
    public string Description { get; set; }
    public string PerformedBy { get; set; }
    public string PerformedByRole { get; set; } // Admin, Teacher, Student
    public DateTime Timestamp { get; set; }
    public string EntityId { get; set; }
    public string EntityType { get; set; } // Class, Student, Teacher, etc.
}

// Application/Features/Dashboard/DTOs/EventDto.cs
namespace Application.Features.Dashboard.DTOs;

public class EventDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string EventType { get; set; } // Exam, Holiday, Meeting, Sports, Cultural
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Location { get; set; }
    public bool IsAllDay { get; set; }
    public string Priority { get; set; } // High, Medium, Low
}

// Application/Features/Dashboard/DTOs/AnnouncementDto.cs
namespace Application.Features.Dashboard.DTOs;

public class AnnouncementDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Priority { get; set; } // Critical, High, Normal, Low
    public string TargetAudience { get; set; } // All, Students, Teachers, Parents, Staff
    public DateTime PublishedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public string PublishedBy { get; set; }
}

// Application/Features/Dashboard/DTOs/HomePageOverviewDto.cs
namespace Application.Features.Dashboard.DTOs;

public class HomePageOverviewDto
{
    public DashboardStatsDto DashboardStats { get; set; }
    public List<ActivityDto> RecentActivities { get; set; }
    public List<EventDto> UpcomingEvents { get; set; }
    public List<AnnouncementDto> Announcements { get; set; }
}
