using API.Extensions;
using API.MiddleWare;
using Application.Settings;
using Infrastructure.Extensions;
using Infrastructure.Helper;
using Infrastructure.Jobs;
using Quartz;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog 
Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7) // keep last 7 days 
                    .CreateLogger();

// Use Serilog instead of default logger
builder.Host.UseSerilog();

builder.Services.AddConfigurationSettings(builder.Configuration);
// Initialize Configuration Helper
ConfigurationHelper.Initialize(builder.Configuration);
builder.Services.AddMediatRAndMasstransit(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddRateLimitingServices();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
// swagger UI...
builder.Services.AddSwaggerGen();

builder.Services.AddJwtAuthentication();
builder.Services.AddApplicationServices();

// Register the background service
builder.Services.AddHostedService<HeartbitTestJob>();

// Configure Quartz.NET
builder.Services.AddQuartz(q =>
{
    // Define the job
    var jobKey = new JobKey("AbsentStudentNotificationJob");
    q.AddJob<AbsentStudentNotificationJob>(opts => opts.WithIdentity(jobKey));

    var jobRubTime = builder.Configuration["JobSettings:ScheduleTime"] ?? "10:00:00";

    // Create a trigger for the job to run daily at 6 AM
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("AbsentStudentNotificationJob-trigger")
        .WithCronSchedule(AbsentNotificationJobSettings.GetCronExpression(jobRubTime))); // every day at 6:00 AM
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

// Initialize database indexes
await app.Services.InitializeDatabaseAsync();

// Use "GlobalLimiter" as rate limiting middleware globally 1000 requests per minute...
app.UseRateLimiter();

// Configure the HTTP request pipeline...
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // add Scalar.AspNetCore...
    // Swagger UI...
    app.UseSwagger();
    app.UseSwaggerUI();
}

// authentication & authorization middlewares...
app.UseAuthentication();
app.UseGlobalExceptionMiddleware();
app.UseAuthMiddleware();
app.UseAuditMiddleware();
app.UseAuthorization();

app.MapControllers();
app.Run();



// Make the implicit Program class accessible to tests
public partial class Program { }
