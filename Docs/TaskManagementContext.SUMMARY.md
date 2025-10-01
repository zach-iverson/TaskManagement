# TaskManagementContext Summary

## Purpose
`TaskManagementContext` is the Entity Framework Core database context for the Task Management API. It manages the application's data access and is responsible for mapping C# entities to database tables.

## Inheritance
This context inherits from `IdentityDbContext`, which means:
- It includes all the tables and configuration needed for ASP.NET Core Identity (users, roles, claims, etc.).
- You can use built-in authentication and authorization features alongside your own entities.

## Key Features
- **DbSet<Models.HumanTask> HumanTasks**: Represents the `HumanTasks` table in the database, allowing CRUD operations on `HumanTask` entities.
- **OnModelCreating**: Configures the model. Here, it:
  - Calls `base.OnModelCreating(modelBuilder)` to ensure Identity tables are set up.
  - Maps the `HumanTask` entity to the table `humantasks` in the `sandbox` schema and sets the primary key to `Id`.

## Example Code
```csharp
public class TaskManagementContext : IdentityDbContext
{
    public TaskManagementContext(DbContextOptions<TaskManagementContext> options) : base(options) { }

    public DbSet<Models.HumanTask> HumanTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Models.HumanTask>().ToTable("humantasks", "sandbox").HasKey(t => t.Id);
    }
}
```

## Why Use IdentityDbContext?
- Adds support for user authentication and authorization.
- Automatically creates tables for users, roles, claims, etc.
- Lets you manage both application data and user data in a single context.

## When to Use
Use `IdentityDbContext` if your application needs user login, registration, and role management. If you only need to manage your own data (no authentication), inherit from `DbContext` instead.

