namespace TodoApi.Models
{
    public interface ITodoRepository
    {
        List<TodoItem> GetAll();
        TodoItem? Get(Guid id);
        void Add(TodoItem todo);
        void Update(TodoItem todo);
        void Delete(Guid id);
    }
}
