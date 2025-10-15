using System.Net.Http.Json;
using System.Text.Json;
using TodoApp.Models;

namespace TodoBlazor.Services
{
    public class TodoService(HttpClient http) : ITodoService
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public async Task<List<TodoItem>> GetAll()
        {
            return await DeserializeTodos("api/todo");
        }

        public async Task<List<TodoItem>> GetAll(string? sortBy = "createdAt", string? sortOrder = "asc")
        {
            var url = $"api/todo?sortBy={sortBy}&sortOrder={sortOrder}";
            var response = await DeserializeTodos(url);

            return response;
        }

        public async Task<List<TodoItem>> GetAll(
            bool? isCompleted = null,
            DateTime? dueFrom = null,
            DateTime? dueTo = null,
            DateTime? createdFrom = null,
            DateTime? createdTo = null,
            string? title = null,
            string? sortBy = "createdAt",
            string? sortOrder = "asc")
        {
            //  check for query params
            var queryParams = new List<string>();

            if (isCompleted.HasValue)
                queryParams.Add($"isCompleted={isCompleted.Value.ToString().ToLower()}");
            if (dueFrom.HasValue)
                queryParams.Add($"dueFrom={dueFrom.Value:yyyy-MM-dd}");
            if (dueTo.HasValue)
                queryParams.Add($"dueTo={dueTo.Value:yyyy-MM-dd}");
            if (createdFrom.HasValue)
                queryParams.Add($"createdFrom={createdFrom.Value:yyyy-MM-dd}");
            if (createdTo.HasValue)
                queryParams.Add($"createdTo={createdTo.Value:yyyy-MM-dd}");
            if (!string.IsNullOrEmpty(title))
                queryParams.Add($"title={Uri.EscapeDataString(title)}");

            queryParams.Add($"sortBy={sortBy}");
            queryParams.Add($"sortOrder={sortOrder}");

            var url = $"api/todo?{string.Join("&", queryParams)}";
            var response = await DeserializeTodos(url);

            return response;

        }

        public async Task<TodoItem> Get(Guid id)
        {
            return await DeserializeTodo($"api/todo/{id}");
        }

        public async Task Add(TodoItem todo) =>
            await http.PostAsJsonAsync("api/todo", todo);

        public async Task Update(TodoItem todo) =>
            await http.PutAsJsonAsync($"api/todo/{todo.Id}", todo);

        public async Task Delete(Guid id) =>
            await http.DeleteAsync($"api/todo/{id}");

        private async Task<List<TodoItem>> DeserializeTodos(string url)
        {
            var response = await http.GetFromJsonAsync<JsonElement>(url);
            
            // The API returns { data: [...], count: ... }
            if (response.TryGetProperty("data", out var dataElement))
            {
                return JsonSerializer.Deserialize<List<TodoItem>>(dataElement, Options) ?? new List<TodoItem>();
            }
            return new List<TodoItem>();
        }

        private async Task<TodoItem> DeserializeTodo(string url)
        {
            return await http.GetFromJsonAsync<TodoItem>(url, Options) ?? new();
        }
    }
}
