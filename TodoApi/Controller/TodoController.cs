using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;

namespace TodoApi.Controller;

[Route("api/[controller]")]
[ApiController]
public class TodoController(ITodoRepository todoRepository) : ControllerBase
{
    //  get all todos
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
            var todos = todoRepository.GetAll().AsQueryable();
            //  query for is completed
            if (isCompleted.HasValue)
                todos = todos.Where(t => t.IsCompleted == isCompleted.Value);
            //  query for due from
            if (dueFrom.HasValue)
                todos = todos.Where(t => t.DueDate.HasValue && t.DueDate.Value >= dueFrom.Value);
            //  query for due to
            if (dueTo.HasValue)
                todos = todos.Where(t => t.DueDate.HasValue && t.DueDate.Value <= dueTo.Value);
            //  query for created from
            if (createdFrom.HasValue)
                todos = todos.Where(t => t.CreatedAt >= createdFrom.Value);
            //  query for created to
            if (createdTo.HasValue)
                todos = todos.Where(t => t.CreatedAt <= createdTo.Value);


            // manage sorting
            bool isDescending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

            //  query for title
            if (!string.IsNullOrWhiteSpace(title))
                todos = todos.Where(t => t.Title.Contains(title, StringComparison.OrdinalIgnoreCase));

            string sortField = sortBy?.ToLower() ?? "createdat";
            //  sort by
            switch (sortField)
            {
                case "duedate":
                    todos = isDescending ? todos.OrderByDescending(t => t.DueDate) : todos.OrderBy(t => t.DueDate);
                    break;
                //  sort by title
                case "title":
                    todos = isDescending ? todos.OrderByDescending(t => t.Title) : todos.OrderBy(t => t.Title);
                    break;
                //  sort by is completed
                case "iscompleted":
                    todos = isDescending
                        ? todos.OrderByDescending(t => t.IsCompleted)
                        : todos.OrderBy(t => t.IsCompleted);
                    break;
                default:
                    todos = isDescending ? todos.OrderByDescending(t => t.CreatedAt) : todos.OrderBy(t => t.CreatedAt);
                    break;
                //  sort by created at
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

    // GET todo by id
    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        try
        {
            var todo = todoRepository.Get(id);
            if (todo == null)
                return NotFound(new { error = "Todo not found" });

            return Ok(todo);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error", detail = ex.Message });
        }
    }

    // POST create todo
    [HttpPost]
    public IActionResult Create(TodoItem todo)
    {
        try
        {
            todoRepository.Add(todo);
            return CreatedAtAction(nameof(Get), new { id = todo.Id }, todo);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error", detail = ex.Message });
        }
    }

    // PUT update todo
    [HttpPut("{id}")]
    public IActionResult Update(Guid id, TodoItem todo)
    {
        try
        {
            if (id != todo.Id)
                return BadRequest(new { error = "Id mismatch" });

            todoRepository.Update(todo);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error", detail = ex.Message });
        }
    }

    // DELETE todo
    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        try
        {
            todoRepository.Delete(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error", detail = ex.Message });
        }
    }
}