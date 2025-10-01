# Lessons from Day 1

## Summary of Today’s Sessions

### What You Asked
- Implement versioned Swagger support.
- Troubleshoot why Swagger UI was not accessible.
- Diagnose CORS issues blocking Swagger requests.
- Explain what CORS is.
- Create a `README.md` covering Docker Compose usage, endpoints, and migrations.
- Provide a consolidated recap to reinforce learning.

### What Was Implemented / Clarified
- Added API versioning explorer and dynamic Swagger generation (plan included creating a `ConfigureSwaggerOptions` to register one doc per version).
- Confirmed expected Swagger access paths (e.g., `https://localhost:7114/swagger` and per-version JSON).
- Identified common causes for Swagger access issues: port mismatch, HTTPS cert trust, missing versioned doc registration.
- Introduced broad development CORS policy (AllowAnyOrigin/Method/Header) to eliminate browser blocking during local testing.
- Delivered a README including: overview, Docker Compose flow, endpoints (CRUD + health), migrations workflow, config notes, and dev vs. prod considerations.

### Key Concepts Learned
- **API Versioning**: `IApiVersionDescriptionProvider` enables enumerating versions for Swagger grouping.
- **Swagger Version Docs**: Each version requires explicit `SwaggerDoc` registration or a configuration class to iterate versions.
- **CORS Basics**: Browser security model (same-origin), preflight (OPTIONS), required headers (`Access-Control-Allow-*`).
- **Typical CORS Pitfalls**: Mixed protocol (HTTP vs HTTPS), mismatched ports, absent headers, blocked preflight.
- **Local Dev Strategy**: Use permissive CORS; tighten origins for production.
- **Migrations Workflow**: `add` → `update` → optionally `remove` for rollback before applying.
- **Operational Flow**: Start containers (`docker-compose up --build`), verify health, explore via Swagger, run EF migrations if schema changes.

### Reinforcement Points
- Swagger issues often stem from missing doc registration or environment (HTTPS trust) rather than code defects.
- CORS errors originate in the browser; server logs may appear normal.
- Versioned APIs future-proof contracts; Swagger grouping makes consumer discovery clearer.
- Documentation (README) accelerates onboarding and reduces repeated setup questions.

### Suggested Mental Model
Request Flow: Browser Swagger UI → Fetch `/swagger/{version}/swagger.json` → If cross-origin: CORS preflight → Server returns policy headers → UI renders operations.

---

### Deep Dive: ModelState, ValidationProblem, and [ApiController]

#### Built-in Features
- **ModelState**: A dictionary that tracks validation errors during model binding.
- **ValidationProblem**: A helper method that generates a standardized 400 response with validation errors.
- **[ApiController]**: Automatically validates models and returns a 400 response if invalid, without requiring manual checks.

#### How It Works
1. **Model Binding**:
   - ASP.NET Core reads the request body, route values, query string, etc., and populates your parameter object (e.g., `CreateTaskRequest`).
   - Validation errors (e.g., from `[Required]`, `[StringLength]`) are collected into `ModelState`.

2. **Automatic Validation**:
   - With `[ApiController]`, the framework automatically validates the model after binding.
   - If invalid, it short-circuits the pipeline and returns a `ValidationProblemDetails` response (RFC 7807 format).

3. **ValidationProblemDetails**:
   - A standardized JSON response for validation errors:
     ```json
     {
       "type": "...",
       "title": "One or more validation errors occurred.",
       "status": 400,
       "errors": {
         "Title": ["The Title field is required."]
       },
       "traceId": "..."
     }
     ```

#### Why Manual Checks Are Optional
- `[ApiController]` already handles validation, so `if (!ModelState.IsValid)` is redundant unless you:
  - Add custom logging.
  - Modify or replace the response.
  - Combine validation with other logic.

#### Customization Options
- **Disable Automatic Validation**:
  ```csharp
  builder.Services.AddControllers()
      .ConfigureApiBehaviorOptions(o =>
      {
          o.SuppressModelStateInvalidFilter = true;
      });
  ```
- **Customize Error Format**:
  - Replace `ProblemDetailsFactory` with a custom implementation.
  - Use middleware or filters to wrap/enrich responses.

#### Example: Custom Logging
```csharp
if (!ModelState.IsValid)
{
    _logger.LogWarning("Invalid payload: {@Errors}",
        ModelState.Where(e => e.Value?.Errors.Count > 0)
                  .ToDictionary(k => k.Key, v => v.Value!.Errors.Select(er => er.ErrorMessage)));
    return ValidationProblem(ModelState);
}
```

#### Summary Flow
1. Request hits the action.
2. Model binding populates the parameter object and `ModelState`.
3. `[ApiController]` validates the model.
4. If invalid, a 400 response is returned automatically.
5. If valid, the action executes.

---

### Deep Dive: Route Template `v{apiVersion:apiVersion}/[controller]`

#### Overview
This route template is part of ASP.NET Core’s attribute-routing system, enhanced by the API Versioning package. It defines versioned, convention-based routes for your controllers and actions.

#### Breaking It Down
- **Literal Text**: `v`
  - The leading `v` is a literal path segment (short for “version”). It must appear exactly in the URL.
  - Example: `/v1/TaskManagement`.

- **Route Parameter**: `{apiVersion:apiVersion}`
  - `{apiVersion}` is a route parameter that captures the version segment of the URL.
  - `:apiVersion` is a custom route constraint provided by the API Versioning package. It ensures the captured value matches a valid API version format (e.g., `1`, `1.0`, `2.1`).
  - The captured value is bound to the `ApiVersion` model and can be accessed programmatically (e.g., `HttpContext.GetRequestedApiVersion()`).

- **Token**: `[controller]`
  - `[controller]` is a route token that ASP.NET Core replaces at runtime with the controller’s name (minus the `Controller` suffix).
  - Example: `TaskManagementController` → `TaskManagement`.

#### Example Resolved Endpoints
- `GET /v1/TaskManagement`
- `GET /v1/TaskManagement/5`
- `POST /v1/TaskManagement`

#### How the `ApiVersion` Attribute Ties In
- `[ApiVersion("1")]` on the controller declares that it supports version `1`.
- The version in the URL segment must match one of the declared versions; otherwise, the framework returns a `404` or `400` depending on configuration.

#### Why Use a Version Route Segment?
- **Explicit and Cache-Friendly**: Segment versioning (e.g., `/v1/...`) is clear and works well with caching.
- **Alternatives**:
  - Query string: `?api-version=1.0`
  - Header: `api-version: 1.0`
  - Media type: Content negotiation.
- **Default Version**:
  - You can configure a default version in `Program.cs`:
    ```csharp
    builder.Services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ReportApiVersions = true;
    });
    ```
  - This allows requests without a version to default to `1.0`.

#### Multiple Versions on One Controller
- You can stack multiple `[ApiVersion]` attributes:
  ```csharp
  [ApiVersion("1")]
  [ApiVersion("2")]
  public class TaskManagementController : ControllerBase
  {
      [HttpGet]
      [MapToApiVersion("2")]
      public IActionResult GetV2() => Ok("This is version 2");
  }
  ```
- The route template stays the same, but the version segment changes (e.g., `/v2/...`).

#### How Matching Works
1. **Routing**:
   - Parses the URL path into segments.
   - Matches `v` (literal), validates the version segment against the `:apiVersion` constraint, and resolves `[controller]`.
2. **API Versioning Middleware**:
   - Validates the requested version against the controller’s declared versions.
   - Dispatches the request or rejects it.

#### Common Pitfalls
- **Typo in the Version Segment**: `/vOne/` → Route not found.
- **Missing `AddApiVersioning`**: The `:apiVersion` constraint won’t be recognized.
- **Controller Renaming**: If you rename the controller but don’t rebuild, the `[controller]` token may resolve incorrectly.
- **Case Sensitivity**: Rare on Windows/Linux defaults but can occur in certain hosting scenarios.

#### Customization
- **Change the Literal**: `api/v{apiVersion:apiVersion}/[controller]`.
- **Make Version Optional**: Add a second route like `[Route("[controller]")]` and enable default version assumptions.

#### Summary
The route template `v{apiVersion:apiVersion}/[controller]` builds versioned, convention-based routes. It combines:
- A literal `v`.
- A validated version segment tied to the API Versioning system.
- A controller name token.

This ensures consumers hit explicit versioned URIs, while the versioning library enforces correctness and supports API evolution over time.

---

### Deep Dive: Configuring TaskManagementContext for PostgreSQL

#### Key Adjustments for PostgreSQL
1. **Schema Handling**:
   - PostgreSQL does not use the `dbo` schema by default. Update `.ToTable("HumanTasks", "dbo")` to `.ToTable("HumanTasks")` unless you explicitly create a schema.
   - Alternatively, specify a custom schema (e.g., `sandbox`) if required.

2. **Connection String**:
   - Ensure your `appsettings.json` contains a valid PostgreSQL connection string:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=TaskManagement;Username=postgres;Password=yourpassword"
     }
     ```

3. **Use the Npgsql Provider**:
   - In `Program.cs`, configure the DbContext to use PostgreSQL:
     ```csharp
     builder.Services.AddDbContext<TaskManagementApi.Database.TaskManagementContext>(options =>
         options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
     ```

#### Step-by-Step Breakdown

1. **Dependency Injection Registration**:
   - `builder.Services.AddDbContext<...>()` registers `TaskManagementContext` with ASP.NET Core’s DI system.
   - This allows the context to be injected into controllers and services.

2. **DbContext Configuration**:
   - The lambda `options => options.UseNpgsql(...)` configures EF Core to use the Npgsql provider for PostgreSQL.

3. **Connection String Retrieval**:
   - `builder.Configuration.GetConnectionString("DefaultConnection")` fetches the connection string from `appsettings.json`.

4. **Service Lifetime**:
   - By default, `AddDbContext` registers the context with a scoped lifetime (one instance per web request).

5. **Provider Initialization**:
   - EF Core uses the Npgsql provider to set up the connection pool, migrations, and database access logic.
   - LINQ queries and EF operations are translated into PostgreSQL-compatible SQL.

6. **Usage in the App**:
   - When `TaskManagementContext` is injected into a controller or service, it provides a ready-to-use context connected to PostgreSQL.

#### Entity Framework Core Concepts

1. **DbContext**:
   - Represents a session with the database, allowing you to query and save data.
   - Manages entity objects during runtime, including change tracking and database connections.

2. **Entities**:
   - C# classes that map to database tables. Each instance represents a row in the table.

3. **DbSet<TEntity>**:
   - Represents a table in the database. Used to query and manipulate data for that entity.

4. **Model Configuration**:
   - Fluent API (e.g., `.ToTable()`) or data annotations configure how classes map to tables, columns, keys, relationships, etc.

5. **Migrations**:
   - EF Core generates and applies database schema changes based on your model changes, keeping the database in sync with your code.

#### Example: TaskManagementContext
```csharp
using Microsoft.EntityFrameworkCore;

namespace TaskManagementApi.Database;

public class TaskManagementContext : DbContext
{
    public TaskManagementContext(DbContextOptions<TaskManagementContext> options) : base(options)
    {
    }

    public DbSet<Models.HumanTask> HumanTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Models.HumanTask>().ToTable("HumanTasks").HasKey(t => t.Id);
    }
}
```

#### Summary
- Configure `TaskManagementContext` to use PostgreSQL by:
  - Removing or updating the schema argument in `.ToTable()`.
  - Using the Npgsql provider with `UseNpgsql`.
  - Providing a valid PostgreSQL connection string.
- EF Core simplifies database interactions by mapping C# objects to database tables and handling schema changes through migrations.
