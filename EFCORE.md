# Mastering Entity Framework Core (EF Core)

## Overview
This document provides a comprehensive guide to becoming an expert in EF Core. It covers foundational concepts, advanced techniques, and practical examples to help you master the framework.

---

## 1. Foundations
### Key Concepts
- **DbContext**: Represents a session with the database.
- **Dependency Injection**: Registers `DbContext` with a scoped lifetime.
- **Change Tracker**: Tracks entity states (Added, Modified, Deleted, Unchanged).
- **Unit of Work**: EF Core implicitly provides this pattern.

### Code Example
```csharp
// Registering DbContext in Program.cs
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Using DbContext in a Controller
public class MyController : ControllerBase
{
    private readonly MyDbContext _context;

    public MyController(MyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetEntities()
    {
        var entities = await _context.MyEntities.ToListAsync();
        return Ok(entities);
    }
}
```

---

## 2. Model Design
### Key Concepts
- **Entity Configuration**: Use Fluent API or Data Annotations.
- **Relationships**: One-to-One, One-to-Many, Many-to-Many.
- **Inheritance**: Table-per-Hierarchy (TPH), Table-per-Type (TPT), Table-per-Concrete-Type (TPC).

### Code Example
```csharp
// Fluent API Configuration
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<MyEntity>()
        .HasKey(e => e.Id);

    modelBuilder.Entity<MyEntity>()
        .HasOne(e => e.RelatedEntity)
        .WithMany(r => r.MyEntities)
        .HasForeignKey(e => e.RelatedEntityId);
}

// Data Annotations
public class MyEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    public int RelatedEntityId { get; set; }
    public RelatedEntity RelatedEntity { get; set; }
}
```

---

## 3. Migrations Mastery
### Key Concepts
- **Add Migrations**: Generate migration files.
- **Apply Migrations**: Update the database schema.
- **Customizing Migrations**: Modify `Up` and `Down` methods.

### Code Example
```bash
# Add a new migration
$ dotnet ef migrations add InitialCreate

# Apply migrations to the database
$ dotnet ef database update
```
```csharp
// Example Migration
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.CreateTable(
        name: "MyEntities",
        columns: table => new
        {
            Id = table.Column<int>(nullable: false)
                .Annotation("SqlServer:Identity", "1, 1"),
            Name = table.Column<string>(maxLength: 100, nullable: false)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_MyEntities", x => x.Id);
        });
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropTable(name: "MyEntities");
}
```

---

## 4. Query Translation
### Key Concepts
- **LINQ to SQL**: Understand client vs server evaluation.
- **Projection**: Shape query results.
- **Raw SQL**: Execute custom SQL queries.

### Code Example
```csharp
// LINQ Query
var entities = await _context.MyEntities
    .Where(e => e.Name.Contains("test"))
    .ToListAsync();

// Projection
var projected = await _context.MyEntities
    .Select(e => new { e.Id, e.Name })
    .ToListAsync();

// Raw SQL
var rawSql = await _context.MyEntities
    .FromSqlRaw("SELECT * FROM MyEntities WHERE Name LIKE '%test%'")
    .ToListAsync();
```

---

## 5. Performance Optimization
### Key Concepts
- **Tracking vs No-Tracking**: Use `AsNoTracking` for read-only queries.
- **Batching**: Minimize database round-trips.
- **Query Splitting**: Avoid cartesian explosions.

### Code Example
```csharp
// No-Tracking Query
var entities = await _context.MyEntities
    .AsNoTracking()
    .ToListAsync();

// Query Splitting
var entitiesWithRelated = await _context.MyEntities
    .Include(e => e.RelatedEntity)
    .AsSplitQuery()
    .ToListAsync();
```

---

## 6. Concurrency & Transactions
### Key Concepts
- **Optimistic Concurrency**: Use rowversion or concurrency tokens.
- **Explicit Transactions**: Manage transactions manually.

### Code Example
```csharp
// Optimistic Concurrency
public class MyEntity
{
    public int Id { get; set; }
    public string Name { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }
}

// Explicit Transaction
using var transaction = await _context.Database.BeginTransactionAsync();
try
{
    _context.MyEntities.Add(new MyEntity { Name = "Test" });
    await _context.SaveChangesAsync();

    transaction.Commit();
}
catch
{
    transaction.Rollback();
    throw;
}
```

---

## 7. Advanced Mapping
### Key Concepts
- **Global Query Filters**: Apply filters to all queries.
- **Temporal Tables**: Track historical data.
- **JSON Columns**: Map JSON data to entities.

### Code Example
```csharp
// Global Query Filter
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<MyEntity>().HasQueryFilter(e => !e.IsDeleted);
}

// JSON Column Mapping
public class MyEntity
{
    public int Id { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; }
}
```

---

## 8. Diagnostics & Observability
### Key Concepts
- **Logging**: Enable detailed logs.
- **Interceptors**: Customize query execution.

### Code Example
```csharp
// Enable Logging
builder.Logging.AddConsole();

// Interceptor Example
public class MyCommandInterceptor : DbCommandInterceptor
{
    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        Console.WriteLine(command.CommandText);
        return base.ReaderExecuting(command, eventData, result);
    }
}

// Register Interceptor
builder.Services.AddDbContext<MyDbContext>(options =>
    options.AddInterceptors(new MyCommandInterceptor()));
```

---

## 9. Testing Strategy
### Key Concepts
- **In-Memory Database**: Use SQLite for testing.
- **Transactional Tests**: Ensure isolation.

### Code Example
```csharp
// In-Memory Database
var options = new DbContextOptionsBuilder<MyDbContext>()
    .UseInMemoryDatabase("TestDb")
    .Options;

using var context = new MyDbContext(options);
context.MyEntities.Add(new MyEntity { Name = "Test" });
await context.SaveChangesAsync();

// Verify
var entity = await context.MyEntities.FirstOrDefaultAsync();
Assert.NotNull(entity);
```

---

## 10. Deployment & Operations
### Key Concepts
- **Migration Scripts**: Generate SQL scripts for DBAs.
- **Zero-Downtime Migrations**: Use expand/contract patterns.

### Code Example
```bash
# Generate SQL Script
$ dotnet ef migrations script -o migration.sql
```

---

## Summary
Mastering EF Core involves understanding its foundational concepts, advanced features, and practical applications. By focusing on the areas outlined above and practicing with real-world scenarios, you can become proficient in building robust, high-performance applications with EF Core.
