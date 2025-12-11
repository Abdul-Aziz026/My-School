using Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.Configure<DBSettings>(builder.Configuration.GetSection("DBSettings"));

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
