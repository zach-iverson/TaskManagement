using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register MVC controllers
builder.Services.AddControllers();
// Explorer for minimal + controllers
builder.Services.AddEndpointsApiExplorer();
// Swashbuckle generator for Swagger UI (/swagger)
builder.Services.AddSwaggerGen(options =>
{
    // You can customize metadata here if desired
    options.SwaggerDoc("v1", new() { Title = "TaskManagement API", Version = "v1" });
});

builder.Services.AddDbContext<TaskManagementApi.Database.TaskManagementContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddScoped<TaskManagementApi.Database.IHumanTaskRepository, TaskManagementApi.Database.HumanTaskRepository>();

builder.Services.AddLogging();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Add CORS policy to allow all origins (for development purposes)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Enforce HTTPS (will redirect http:// to https://)
app.UseHttpsRedirection();

// Swagger (leave enabled always for now)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.DocumentTitle = "TaskManagement API Docs";
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManagement API v1");
    // If hosting behind a reverse proxy with a base path, set c.SwaggerEndpoint relative to that base.
});

// Simple ping endpoint to verify the app is reachable quickly
app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

// Use CORS middleware
app.UseCors();

// Map attribute-routed controllers (must be after building services)
app.MapControllers();

app.Run();