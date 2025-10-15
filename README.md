# Todo Application

A full-stack todo app.

## Architecture

This solution consists of three main components:

- **TodoApi** - ASP.NET Core Web API backend
- **TodoBlazor** - Blazor WebAssembly frontend
- **TodoApi.Tests** - Unit tests for the API

## Features

### Backend (API)

- Full CRUD operations for todo items
- Multiple sorting options (by title, due date, completion status, creation date)

## Technology Stack

- **Backend**: ASP.NET Core 9.0 Web API
- **Frontend**: Blazor

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- A modern web browser
- Code editor (Visual Studio, VS Code, or Rider)

### Docker Setup

To run the app with docker simply run the command:

```
docker compose up --build
```

This will start the app on localhost:5000.

### Required Variables

- `API_BASE_URL`: The base URL for the Todo API (e.g., `http://localhost:5000`) in docker
- `ASPNETCORE_ENVIRONMENT`: The environment name (Development, Staging, Production) // Development is Default

## Getting Started Locally instead

### 1. Clone the Repository

```bash
git clone https://github.com/roychowd/todos-.git
cd todos-
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Run the Application

#### Option A: Run Both Projects Simultaneously

```bash
dotnet run --project TodoApi
# In another terminal:
dotnet run --project TodoBlazor
```

### 4. Access the Application

- **Frontend**: http://localhost:5221 (or the port shown in console)
- **API**: http://localhost:5000 (or the port shown in console)

## Project Structure

```
TodoSoln/
├── TodoApi/                    # Web API Backend
│   ├── Controller/
│   │   └── TodoController.cs   # REST API endpoints
│   ├── Models/
│   │   ├── TodoItem.cs         # Data model
│   │   └── ITodoRepository.cs  # Repository interface
│   ├── Repositories/
│   │   └── TodoRepository.cs   # Data access layer
│   ├── App_Data/
│   │   └── todos_data.json     # Data storage
│   └── Program.cs              # Application startup
├── TodoBlazor/                 # Blazor WebAssembly Frontend
│   ├── Pages/
│   │   ├── Home.razor          # Main todo list page
│   │   ├── AddTodo.razor       # Add new todo page
│   │   ├── EditToDo.Razor      # Edit todo page
│   │   └── DetailsPage.Razor   # Todo details page
│   ├── Services/
│   │   ├── ITodoService.cs     # Service interface
│   │   └── TodoService.cs      # API communication
│   ├── Models/
│   │   └── TodoItem.cs         # Shared data model
│   └── Layout/                 # UI layout components
├── TodoApi.Tests/              # Unit Tests
│   └── UnitTest1.cs            # Controller tests
└── README.md                   # This file
```

## API Endpoints

### Base URL: `http://localhost:5000/api/todo`

| Method | Endpoint | Description     | Parameters                                                                                    |
| ------ | -------- | --------------- | --------------------------------------------------------------------------------------------- |
| GET    | `/`      | Get all todos   | `sortBy`, `sortOrder`, `isCompleted`, `dueFrom`, `dueTo`, `createdFrom`, `createdTo`, `title` |
| GET    | `/{id}`  | Get todo by ID  | `id` (Guid)                                                                                   |
| POST   | `/`      | Create new todo | TodoItem in body                                                                              |
| PUT    | `/{id}`  | Update todo     | `id` (Guid), TodoItem in body                                                                 |
| DELETE | `/{id}`  | Delete todo     | `id` (Guid)                                                                                   |

### Query Parameters for GET `/`

- `sortBy`: `createdAt`, `dueDate`, `title`, `isCompleted`
- `sortOrder`: `asc`, `desc`
- `isCompleted`: `true`, `false`
- `dueFrom`: Date (YYYY-MM-DD)
- `dueTo`: Date (YYYY-MM-DD)
- `createdFrom`: Date (YYYY-MM-DD)
- `createdTo`: Date (YYYY-MM-DD)
- `title`: String (partial match)

### Example API Calls

```bash
# Get all todos sorted by creation date
GET /api/todo?sortBy=createdAt&sortOrder=desc

# Get completed todos due this week
GET /api/todo?isCompleted=true&dueFrom=2025-10-12&dueTo=2025-10-18

# Search todos by title
GET /api/todo?title=meeting&sortBy=dueDate
```

## Running Tests

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests for specific project
dotnet test TodoApi.Tests/
```

## Data Model

### TodoItem

```csharp
public class TodoItem
{
    public Guid Id { get; set; }                    // Unique identifier
    public string Title { get; set; }               // Todo title (required, max 100 chars)
    public string? Description { get; set; }        // Optional description
    public DateTime? DueDate { get; set; }          // Optional due date
    public bool IsCompleted { get; set; }           // Completion status
    public DateTime CreatedAt { get; set; }         // Creation timestamp
}
```

### Design Choices and Assumptions

I decided to go for a simple C# Web API with Blazor since the role requires C# and I believe it was a good demonstration of my skills. I assumed for filtering createdAt and dueDate should be taken into consideration too so I added range filters for both those values. Testing for these filters was the hard part.  I created unit tests to thoroughly test my application - specifically for filters. I tested around 100 Todo items and checking for DueDate (from and to), CreatedAt, and isCompleted where true. The components on the frontend are pretty basic - regular bootstrap components with a blazor template.

### AI USE
For full transparency - I did use Ai to generate some helper tests for the other basic endpoints and to help write this readme and documentation. 
