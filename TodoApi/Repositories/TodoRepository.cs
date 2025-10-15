using System.Text.Json;
using TodoApi.Models;

namespace TodoApi.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private const string FilePath = "App_Data/todos_data.json";

        public TodoRepository()
        {
            var directory = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                //  create directory
                Directory.CreateDirectory(directory);
        }

        //  load the todos from the json file
        private List<TodoItem> Load()
        {
            if (!File.Exists(FilePath))
                return new List<TodoItem>();

            try
            {
                var json = File.ReadAllText(FilePath);
                //  if list is empty then we need to return an empty list
                if (string.IsNullOrEmpty(json))
                    return new List<TodoItem>();

                var todos = JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
                return todos;
            }
            catch
            {
                return new List<TodoItem>();
            }
        }

        //  save the todos to the json file
        private void Save(List<TodoItem> todos)
        {
            var json = JsonSerializer.Serialize(todos, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        //  get all the todos
        public List<TodoItem> GetAll() => Load();

        //  get a todo by id
        public TodoItem? Get(Guid id) => Load().FirstOrDefault(t => t.Id == id);

        //  add a todo
        public void Add(TodoItem todo)
        {
            var todos = Load();
            todos.Add(todo);
            Save(todos);
        }

        //  update a todo
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

        //  delete a todo
        public void Delete(Guid id)
        {
            var todos = Load().Where(t => t.Id != id).ToList();
            Save(todos);
        }
    }
}