using System.Net.Http.Json;
using TodoApp.Models;


namespace TodoBlazor.Services
{
    public class TodoService : ITodoService
    {
        private readonly HttpClient _http;

        public TodoService(HttpClient http) => _http = http;

        public async Task<List<TodoItem>> GetAll() =>
            await _http.GetFromJsonAsync<List<TodoItem>>("api/todo") ?? new();

        public async Task<TodoItem> Get(Guid id) =>
            await _http.GetFromJsonAsync<TodoItem>($"api/todo/{id}") ?? new();

        public async Task Add(TodoItem todo) =>
            await _http.PostAsJsonAsync("api/todo", todo);

        public async Task Update(TodoItem todo) =>
            await _http.PutAsJsonAsync($"api/todo/{todo.Id}", todo);

        public async Task Delete(Guid id) =>
            await _http.DeleteAsync($"api/todo/{id}");
    }
}
