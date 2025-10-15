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

            //  add query params
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

            //  add sort params
            queryParams.Add($"sortBy={sortBy}");
            queryParams.Add($"sortOrder={sortOrder}");

            //  build url
            var url = $"api/todo?{string.Join("&", queryParams)}";

            //  deserialize todos
            var response = await DeserializeTodos(url);

            return response;
        }

        //  get todo by id
        public async Task<TodoItem> Get(Guid id)
        {
            return await DeserializeTodo($"api/todo/{id}");
        }

        //  add todo
        public async Task Add(TodoItem todo) =>
            await http.PostAsJsonAsync("api/todo", todo);

        //  update todo
        public async Task Update(TodoItem todo) =>
            await http.PutAsJsonAsync($"api/todo/{todo.Id}", todo);

        //  delete todo
        public async Task Delete(Guid id) =>
            await http.DeleteAsync($"api/todo/{id}");

        //  deserialize todos
        private async Task<List<TodoItem>> DeserializeTodos(string url)
        {
            var response = await http.GetFromJsonAsync<JsonElement>(url);

            // The API returns { data: [...], count: ... }
            if (response.TryGetProperty("data", out var dataElement))
            {
                return dataElement.Deserialize<List<TodoItem>>(Options) ?? new List<TodoItem>();
            }

            return new List<TodoItem>();
        }

        private async Task<TodoItem> DeserializeTodo(string url)
        {
            return await http.GetFromJsonAsync<TodoItem>(url, Options) ?? new();
        }
    }
}
