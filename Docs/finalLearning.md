# 📚 Project Learnings Summary: Task Management API

This document summarizes the core technical concepts and best practices mastered during the development of the secure, scalable, multi-user Task Management API, built using ASP.NET Core and C#.

## 1. Architecture and Object-Oriented (OO) Design
- **Dependency Injection (DI)**
  - Mastered constructor injection and service registration (Scoped lifetime) in Program.cs.
  - Ensured testability and decoupling by injecting services (DbContext, ITaskRepository).
- **Repository Pattern**
  - Implemented the data access logic behind an interface (ITaskRepository).
  - Separated the controller's business logic from the database's persistence logic, enabling easy database or ORM changes.
- **Data Transfer Objects (DTOs)**
  - Created distinct models for reading (TaskReadDto), creating (TaskCreateDto), and updating data.
  - Enforced a strict API Contract, prevented over-posting, and ensured data is secure and tailored for the client.
- **AutoMapper**
  - Used a library to automate the repetitive mapping between DTOs and EF Core Entities.
  - Significantly reduced boilerplate code in controllers and service layers, improving maintainability.
- **Unit Testing**
  - Set up an xUnit project and used mocking (Moq) to test the ITaskRepository implementation.
  - Verified business logic and authorization rules without requiring a live database connection.

## 2. Entity Framework Core (EF Core) Mastery
- **DbContext & DbSet**
  - Defined the application's database session and mapping of C# entities to SQL tables.
  - Used the DbContext instance scoped per request for efficient unit of work management.
- **DB Migrations**
  - Managed the evolution of the SQL Server schema using dotnet ef migrations add/update.
  - Created and applied migrations for both custom entities and the ASP.NET Core Identity schema.
- **One-to-Many Relationships**
  - Correctly modeled AppUser → Task and Category → Task relationships.
  - Enforced referential integrity in the SQL database using foreign keys and navigation properties.
- **Owned Entities (Value Objects)**
  - Used OwnsOne() to model complex object properties like AuditInfo within the Task entity.
  - Promoted cleaner domain modeling by treating value objects as part of the parent entity's table.
- **Data Projection (.Select())**
  - Utilized LINQ's .Select() operator to map data directly to DTOs.
  - Optimized data retrieval by generating SQL that fetches only the columns required for the DTO, avoiding excessive data transfer.
- **Global Query Filters**
  - Defined a filter in DbContext.OnModelCreating (e.g., HasQueryFilter(t => !t.IsDeleted)).
  - Implemented Soft Delete uniformly across the entire application, ensuring deleted items are automatically excluded from all queries.

## 3. ASP.NET Core and Security Fundamentals
- **Middleware Pipeline**
  - Understood the ordered sequence of components that process an HTTP request.
  - Correctly ordered UseAuthentication() and UseAuthorization() to ensure identity is established before permissions are checked.
- **Custom Middleware**
  - Created a component to intercept and handle unhandled exceptions globally.
  - Returned consistent 500 Internal Server Error responses, preventing sensitive server details from reaching the client.
- **Authentication (JWT)**
  - Integrated ASP.NET Core Identity for user management and configured JWT Bearer Authentication.
  - Upon login, a stateless token is generated and validated on subsequent requests, attaching the user identity (Claims Principal) to the request context.
- **Authorization**
  - Used the [Authorize] attribute and performed granular user ID checks in the Repository layer.
  - Enforced the fundamental security rule: users can only access and modify their own tasks.
- **Paging & Filtering**
  - Implemented query parameters to control data returned from the main GET endpoint.
  - Essential for scalability and efficient client-side data handling.
- **Environment Configuration**
  - Managed settings (like connection strings and logging levels) using appsettings.json and environment-specific overrides (appsettings.Production.json).
  - Allowed for easy configuration switching between local development and production deployment.

