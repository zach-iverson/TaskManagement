# TaskManagement API

## Overview
The TaskManagement API is a RESTful service for managing human tasks. It provides endpoints to create, retrieve, update, and delete tasks, as well as mark tasks as complete. The API supports versioning and includes Swagger documentation for easy exploration.

## Features
- **Task Management**: CRUD operations for tasks.
- **API Versioning**: Supports multiple API versions.
- **Swagger Documentation**: Interactive API documentation available at `/swagger`.
- **CORS Support**: Configured to allow cross-origin requests for development.
- **Database Integration**: Uses PostgreSQL for data persistence.

---

## Prerequisites
- **Docker**: Ensure Docker is installed and running.
- **.NET SDK**: Required for running migrations or local development.
- **PostgreSQL**: The API connects to a PostgreSQL database (configured in `appsettings.json`).

---

## Running the API with Docker Compose

1. **Navigate to the Project Directory**:
   ```bash
   cd /Users/ziverso/Documents/svc/zdi-github/TaskManagementApi
   ```

2. **Start the Services**:
   Use Docker Compose to start the API and database:
   ```bash
   docker-compose up --build
   ```

3. **Access the API**:
   - Swagger UI: [http://localhost:5000/swagger](http://localhost:5000/swagger)
   - Health Check: [http://localhost:5000/healthz](http://localhost:5000/healthz)

4. **Stop the Services**:
   To stop the containers, press `Ctrl+C` and run:
   ```bash
   docker-compose down
   ```

---

## API Endpoints

### Base URL
- **Development**: `http://localhost:5000`

### Endpoints

#### Task Management
| Method | Endpoint                  | Description                     | Query Parameters |
|--------|---------------------------|---------------------------------|------------------|
| GET    | `/v1/TaskManagement`      | Retrieve paginated and filtered tasks. | `pageNumber`, `pageSize`, `search`, `isComplete` |
| GET    | `/v1/TaskManagement/{id}` | Retrieve a task by ID.          |                  |
| POST   | `/v1/TaskManagement`      | Create a new task.              |                  |
| PUT    | `/v1/TaskManagement/{id}` | Update an existing task.        |                  |
| PATCH  | `/v1/TaskManagement/{id}/complete` | Mark a task as complete. |                  |
| DELETE | `/v1/TaskManagement/{id}` | Delete a task.                  |                  |

**Pagination and Filtering**
- `pageNumber` (int, default: 1): The page number to retrieve.
- `pageSize` (int, default: 10): The number of tasks per page.
- `search` (string, optional): Filter tasks by title or description containing this text.
- `isComplete` (bool, optional): Filter tasks by completion status.

**Example Request:**
```
GET /v1/TaskManagement?pageNumber=2&pageSize=5&search=meeting&isComplete=false
```

**Response:**
```
{
  "items": [ ... ],
  "totalCount": 42,
  "pageNumber": 2,
  "pageSize": 5
}
```

#### Health Check
| Method | Endpoint   | Description          |
|--------|------------|----------------------|
| GET    | `/healthz` | Check API health.    |

---

## Running Migrations

1. **Navigate to the Project Directory**:
   ```bash
   cd /Users/ziverso/Documents/svc/zdi-github/TaskManagementApi
   ```

2. **Add a New Migration**:
   ```bash
   dotnet ef migrations add <MigrationName> --project TaskManagementApi.csproj
   ```

3. **Apply Migrations to the Database**:
   ```bash
   dotnet ef database update --project TaskManagementApi.csproj
   ```

4. **Remove a Migration (if needed)**:
   ```bash
   dotnet ef migrations remove --project TaskManagementApi.csproj
   ```

---

## Configuration

### appsettings.json
Update the `appsettings.json` file to configure the database connection string:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=TaskManagement;Username=postgres;Password=yourpassword"
}
```

---

## Development

### Running Locally
1. **Restore Dependencies**:
   ```bash
   dotnet restore
   ```

2. **Run the Application**:
   ```bash
   dotnet run --project TaskManagementApi.csproj
   ```

3. **Access the API**:
   - Swagger UI: [https://localhost:7114/swagger](https://localhost:7114/swagger)
   - Health Check: [https://localhost:7114/healthz](https://localhost:7114/healthz)

---

## Key Learnings & Achievements

- **Architecture & Design Principles (C# / OO)**
  - Mastered Dependency Injection (DI): Registered services (Scoped, Singleton) and injected dependencies (DbContext, repositories) for decoupled, testable code.
  - Implemented the Repository Pattern: Separated concerns into Controllers (HTTP), Repositories (data access), and Entities/DTOs (data structure).

- **Entity Framework Core (EF Core) Deep Dive**
  - Learned DbContext, DbSet, and CLI migrations to manage database schema from C# code.
  - Modeled relationships: Managed one-to-many relationships (AppUser → Task, Category → Task) with foreign keys for data integrity.
  - Modeled Value Objects: Used Owned Entities (e.g., AuditInfo) for clean domain modeling.
  - Optimized Performance: Used .Select() projection to fetch only needed columns, improving query efficiency.
  - Enforced Data Integrity: Applied Global Query Filters for soft deletes, ensuring business rules are enforced automatically.

- **Security and ASP.NET Core Pipeline**
  - Integrated Authentication & JWT: Used ASP.NET Core Identity and JWTs for stateless authentication.
  - Implemented Authorization: Used [Authorize] for class-level and object-level security, restricting access to user-owned tasks.
  - Understood Middleware: Ordered UseAuthentication, UseAuthorization, and custom error handling for safe, consistent request processing.

---


---

## License
This project is licensed under the MIT License.
