using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoRepository _todoRepository;

        public TodoController()
        {
            _todoRepository = new TodoRepository();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_todoRepository.GetAll());
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
}