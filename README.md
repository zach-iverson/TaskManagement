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
| Method | Endpoint                  | Description                     |
|--------|---------------------------|---------------------------------|
| GET    | `/v1/TaskManagement`      | Retrieve all tasks.             |
| GET    | `/v1/TaskManagement/{id}` | Retrieve a task by ID.          |
| POST   | `/v1/TaskManagement`      | Create a new task.              |
| PUT    | `/v1/TaskManagement/{id}` | Update an existing task.        |
| PATCH  | `/v1/TaskManagement/{id}/complete` | Mark a task as complete. |
| DELETE | `/v1/TaskManagement/{id}` | Delete a task.                  |

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

## Notes
- Ensure the PostgreSQL database is running before starting the API.
- Use the Swagger UI to explore and test the API endpoints interactively.
- For production, update the CORS policy to restrict allowed origins.

---

## License
This project is licensed under the MIT License.
