using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;

namespace TodoApi.Controller;

[Route("api/[controller]")]
[ApiController]
public class TodoController : ControllerBase
{
    private readonly TodoRepository _todoRepository = new();

    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] bool? isCompleted = null,
        [FromQuery] DateTime? dueFrom = null,
        [FromQuery] DateTime? dueTo = null,
        [FromQuery] DateTime? createdFrom = null,
        [FromQuery] DateTime? createdTo = null,
        [FromQuery] string? title = null,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortOrder = "asc")
    {
        try
        {
            // filtering
            var todos = _todoRepository.GetAll().AsQueryable();
            if (isCompleted.HasValue)
                todos = todos.Where(t => t.IsCompleted == isCompleted.Value);
            if (dueFrom.HasValue)
                todos = todos.Where(t => t.DueDate.HasValue && t.DueDate.Value >= dueFrom.Value);
            if (dueTo.HasValue)
                todos = todos.Where(t => t.DueDate.HasValue && t.DueDate.Value <= dueTo.Value);
            if (createdFrom.HasValue)
                todos = todos.Where(t => t.CreatedAt >= createdFrom.Value);
            if (createdTo.HasValue)
                todos = todos.Where(t => t.CreatedAt <= createdTo.Value);


            // manage sorting
            bool isDescending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(title))
                todos = todos.Where(t => t.Title.Contains(title, StringComparison.OrdinalIgnoreCase));

            string sortField = sortBy?.ToLower() ?? "createdat";
            switch (sortField)
            {
                case "duedate":
                    todos = isDescending ? todos.OrderByDescending(t => t.DueDate) : todos.OrderBy(t => t.DueDate);
                    break;
                case "title":
                    todos = isDescending ? todos.OrderByDescending(t => t.Title) : todos.OrderBy(t => t.Title);
                    break;
                case "iscompleted":
                    todos = isDescending ? todos.OrderByDescending(t => t.IsCompleted) : todos.OrderBy(t => t.IsCompleted);
                    break;
                default:
                    todos = isDescending ? todos.OrderByDescending(t => t.CreatedAt) : todos.OrderBy(t => t.CreatedAt);
                    break;
                case "createdat":
                    todos = isDescending ? todos.OrderByDescending(t => t.CreatedAt) : todos.OrderBy(t => t.CreatedAt);
                    break;
            }

            return Ok(new
            {
                data = todos.ToList(),
                count = todos.Count()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = "Internal server error",
                detail = ex.Message
            });
        }
    }


    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        var todo = _todoRepository.Get(id);
        if (todo == null)
        {
            return NotFound();
        }
        return Ok(todo);
    }

    [HttpPost]
    public IActionResult Create(TodoItem todo)
    {
        _todoRepository.Add(todo);
        return CreatedAtAction(nameof(Get), new { id = todo.Id }, todo);
    }

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, TodoItem todo)
    {
        if (id != todo.Id)
        {
            return BadRequest();
        }
        _todoRepository.Update(todo);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        _todoRepository.Delete(id);
        return NoContent();
    }
}