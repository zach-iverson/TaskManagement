using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManagementApi.Database;
using TaskManagementApi.Database.Security;
using TaskManagementApi.Middleware;

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
    // Add JWT Bearer support
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid JWT token. Example: Bearer eyJhbGci..."
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<TaskManagementContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<TaskManagementContext>();

// Add authentication with JWT Bearer tokens
// Add authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddScoped<IHumanTaskRepository, HumanTaskRepository>();

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

app.UseMiddleware<ExceptionHandlingMiddleware>();
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

// Use authentication & authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map attribute-routed controllers (must be after building services)
app.MapControllers();

app.Run();