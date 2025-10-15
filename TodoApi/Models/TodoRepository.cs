using System.Text.Json;

namespace TodoApp.Models
{
    public class TodoRepository
    {
        private readonly string filePath = "App_Data/todos_data.json";

        public TodoRepository()
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                //  create directory
                Directory.CreateDirectory(directory);
        }

        private List<TodoItem> Load()
        {
            if (!File.Exists(filePath))
                return new List<TodoItem>();

            try
            {
                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
            }
            catch
            {
                return new List<TodoItem>();
            }
        }

        private void Save(List<TodoItem> todos)
        {
            var json = JsonSerializer.Serialize(todos, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public List<TodoItem> GetAll() => Load();

        public TodoItem? Get(Guid id) => Load().FirstOrDefault(t => t.Id == id);

        public void Add(TodoItem todo)
        {
            var todos = Load();
            todos.Add(todo);
            Save(todos);
        }

        public void Update(TodoItem todo)
        {
            var todos = Load();
            var index = todos.FindIndex(t => t.Id == todo.Id);
            if (index != -1)
            {
                todos[index] = todo;
                Save(todos);
            }
        }

        public void Delete(Guid id)
        {
            var todos = Load().Where(t => t.Id != id).ToList();
            Save(todos);
        }
        public void setToCompleted(Guid id)
        {
            var todos = Load();
            var index = todos.FindIndex(t => t.Id == id);
            if (index != -1)
            {
                todos[index].IsCompleted = true;
                Save(todos);
            }
        }

        public void setToInProgress(Guid id)
        {
            var todos = Load();
            var index = todos.FindIndex(t => t.Id == id);
            if (index != -1)
            {
                todos[index].IsCompleted = false;
                Save(todos);
            }
        }
    }
}