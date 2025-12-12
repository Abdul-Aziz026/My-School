using Serilog;
using Domain.Interfaces;
using Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);


// Configure Serilog 
Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7) // keep last 7 days 
                    .CreateLogger(); 

// Use Serilog instead of default logger
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.Configure<DBSettings>(builder.Configuration.GetSection("DBSettings"));
builder.Services.AddSingleton<IDatabaseContext, DatabaseContext>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
