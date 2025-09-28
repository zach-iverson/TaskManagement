# Mastering ASP.NET Framework

## Overview
This document provides a comprehensive guide to becoming an expert in ASP.NET (Core and Classic). It covers foundational concepts, advanced techniques, and practical examples to help you master the framework.

---

## 1. Platform Fundamentals
### Key Concepts
- **Hosting Model**: ASP.NET Core uses Kestrel as the default web server, but it can be hosted behind IIS or Nginx.
- **Request Lifecycle**: Requests pass through middleware in the pipeline before reaching endpoints.
- **HttpContext**: Provides access to request/response data.

### Code Example
```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Middleware pipeline
app.Use(async (context, next) =>
{
    Console.WriteLine("Request: " + context.Request.Path);
    await next();
    Console.WriteLine("Response: " + context.Response.StatusCode);
});

app.MapGet("/hello", () => "Hello, World!");

app.Run();
```

---

## 2. Configuration & Environments
### Key Concepts
- **Configuration Providers**: JSON files, environment variables, user secrets, etc.
- **Options Pattern**: Bind configuration to strongly-typed objects.
- **Environment-Specific Config**: Use `appsettings.Development.json` for development.

### Code Example
```csharp
// appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "MyAppSettings": {
    "FeatureFlag": true
  }
}

// Program.cs
builder.Services.Configure<MyAppSettings>(builder.Configuration.GetSection("MyAppSettings"));

// Accessing configuration in a controller
public class MyController : ControllerBase
{
    private readonly MyAppSettings _settings;

    public MyController(IOptions<MyAppSettings> settings)
    {
        _settings = settings.Value;
    }

    [HttpGet]
    public IActionResult GetFeatureFlag()
    {
        return Ok(_settings.FeatureFlag);
    }
}

// Strongly-typed settings class
public class MyAppSettings
{
    public bool FeatureFlag { get; set; }
}
```

---

## 3. Dependency Injection
### Key Concepts
- **Service Lifetimes**: Transient, Scoped, Singleton.
- **Avoid Capturing Scoped Services**: Scoped services should not be injected into singletons.
- **Decorator Pattern**: Wrap services with additional behavior.

### Code Example
```csharp
// Registering services
builder.Services.AddTransient<IMyService, MyService>();
builder.Services.AddScoped<IMyScopedService, MyScopedService>();
builder.Services.AddSingleton<IMySingletonService, MySingletonService>();

// Using services in a controller
public class MyController : ControllerBase
{
    private readonly IMyService _service;

    public MyController(IMyService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetData()
    {
        return Ok(_service.GetData());
    }
}

// Example service
public interface IMyService
{
    string GetData();
}

public class MyService : IMyService
{
    public string GetData() => "Hello from MyService!";
}
```

---

## 4. Middleware Mastery
### Key Concepts
- **Custom Middleware**: Create reusable middleware components.
- **Short-Circuiting**: Stop the pipeline early if needed.
- **Exception Handling**: Centralized error handling.

### Code Example
```csharp
// Custom Middleware
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine("Handling request: " + context.Request.Path);
        await _next(context);
        Console.WriteLine("Finished handling request.");
    }
}

// Registering middleware
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseMiddleware<LoggingMiddleware>();
app.MapGet("/", () => "Hello, Middleware!");

app.Run();
```

---

## 5. Routing & Endpoints
### Key Concepts
- **Conventional Routing**: Define routes in `Startup.cs`.
- **Attribute Routing**: Use attributes on controllers and actions.
- **Endpoint Routing**: Centralized routing in ASP.NET Core.

### Code Example
```csharp
// Attribute Routing
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(new[] { "Product1", "Product2" });

    [HttpGet("{id}")]
    public IActionResult GetById(int id) => Ok(new { Id = id, Name = "Product" + id });
}

// Endpoint Routing
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/hello", () => "Hello, World!");
app.MapControllers();

app.Run();
```

---

## 6. Authentication & Authorization
### Key Concepts
- **Schemes**: Cookie, JWT, OAuth2.
- **Policies**: Define fine-grained access rules.
- **Claims Transformation**: Modify claims at runtime.

### Code Example
```csharp
// Adding authentication
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/login";
    });

// Adding authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Role", "Admin"));
});

// Protecting endpoints
[Authorize(Policy = "AdminOnly")]
[HttpGet("admin")]
public IActionResult AdminEndpoint() => Ok("Welcome, Admin!");
```

---

## 7. Observability
### Key Concepts
- **Logging**: Structured logging with Serilog.
- **Tracing**: Distributed tracing with OpenTelemetry.
- **Metrics**: Collect performance metrics with Prometheus.

### Code Example
```csharp
// Adding Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Adding OpenTelemetry
builder.Services.AddOpenTelemetryTracing(builder =>
{
    builder.AddAspNetCoreInstrumentation()
           .AddConsoleExporter();
});

// Adding Prometheus
builder.Services.AddMetrics();
app.UseMetrics();
```

---

## Summary
Mastering ASP.NET involves understanding its foundational concepts, advanced features, and practical applications. By focusing on the areas outlined above and practicing with real-world scenarios, you can become proficient in building robust, high-performance applications with ASP.NET.
