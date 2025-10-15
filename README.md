# Todo Application

A full-stack todo management application built with ASP.NET Core Web API and Blazor WebAssembly, featuring comprehensive filtering, sorting, and CRUD operations.

## Architecture

This solution consists of three main components:

- **TodoApi** - ASP.NET Core Web API backend
- **TodoBlazor** - Blazor WebAssembly frontend
- **TodoApi.Tests** - Unit tests for the API

## Features

### Backend (API)
- Full CRUD operations for todo items
- Advanced filtering (by title, completion status, due date, creation date)
- Multiple sorting options (by title, due date, completion status, creation date)
- Data validation and error handling
- JSON file-based data persistence
- Comprehensive unit test coverage

### Frontend (Blazor)
- filtering and sorting
- Add new todos with form validation
- Edit existing todos
- View detailed todo information

- Delete todos with confirmation
## Technology Stack

- **Backend**: ASP.NET Core 9.0 Web API
- **Frontend**: Blazor 
- **Testing**: xUnit, Moq
- **Data Storage**: JSON file-based persistence
- **Development**: .NET 9.0

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- A modern web browser
- Code editor (Visual Studio, VS Code, or Rider)

## Getting Started

### 1. Clone the Repository
```bash
git clone <repository-url>
cd TodoSoln
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

#### Option B: Run from Visual Studio
1. Open `TodoSolution.sln` in Visual Studio
2. Set both `TodoApi` and `TodoBlazor` as startup projects
3. Press F5 to run

### 4. Access the Application
- **Frontend**: http://localhost:5221 (or the port shown in console)
- **API**: http://localhost:5028 (or the port shown in console)

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

## 🔌 API Endpoints

### Base URL: `http://localhost:5221/api/todo`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| GET | `/` | Get all todos | `sortBy`, `sortOrder`, `isCompleted`, `dueFrom`, `dueTo`, `createdFrom`, `createdTo`, `title` |
| GET | `/{id}` | Get todo by ID | `id` (Guid) |
| POST | `/` | Create new todo | TodoItem in body |
| PUT | `/{id}` | Update todo | `id` (Guid), TodoItem in body |
| DELETE | `/{id}` | Delete todo | `id` (Guid) |

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
GET /api/todo?isCompleted=true&dueFrom=2024-10-12&dueTo=2025-10-18

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
`
