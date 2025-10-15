using TodoApp.Models;

namespace TodoBlazor.Services
{
    public interface ITodoService
    {
        Task<List<TodoItem>> GetAll();
        Task<List<TodoItem>> GetAll(string? sortBy = "createdAt", string? sortOrder = "asc");
        Task<List<TodoItem>> GetAll(
            bool? isCompleted = null,
            DateTime? dueFrom = null,
            DateTime? dueTo = null,
            DateTime? createdFrom = null,
            DateTime? createdTo = null,
            string? title = null,
            string? sortBy = "createdAt",
            string? sortOrder = "asc");
        Task<TodoItem> Get(Guid id);
        Task Add(TodoItem todo);
        Task Update(TodoItem todo);
        Task Delete(Guid id);
    }
}