using TodoApp.Models;

namespace TodoBlazor.Services
{
    public interface ITodoService
    {
        Task<List<TodoItem>> GetAll();
        Task<TodoItem> Get(Guid id);
        Task Add(TodoItem todo);
        Task Update(TodoItem todo);
        Task Delete(Guid id);
    }
}