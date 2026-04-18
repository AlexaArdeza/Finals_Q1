using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Security.Cryptography;
using System.Text;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private static readonly List<Todo> _todos = new List<Todo>();

        [HttpGet]
        public IActionResult GetAll() => Ok(_todos);

        [HttpPost]
        public IActionResult Create([FromBody] Todo newTodo)
        {
            if (string.IsNullOrWhiteSpace(newTodo.Title))
                return BadRequest("Title cannot be empty.");

            newTodo.Id = Guid.NewGuid();
            newTodo.CreatedAt = DateTime.UtcNow;

            // Blockchain Logic: Link to previous hash
            var lastTodo = _todos.LastOrDefault();
            newTodo.PreviousHash = lastTodo?.Hash ?? "00000000000000000000000000000000";
            newTodo.Hash = CalculateHash(newTodo);

            _todos.Add(newTodo);
            return CreatedAtAction(nameof(GetAll), new { id = newTodo.Id }, newTodo);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] Todo updatedTodo)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo == null) return NotFound();

            if (string.IsNullOrWhiteSpace(updatedTodo.Title))
                return BadRequest("Title cannot be empty.");

            todo.Title = updatedTodo.Title;
            todo.Completed = updatedTodo.Completed;
            
            // Recalculate hash because content changed (Chain will break if we don't handle it carefully, but let's keep it simple)
            todo.Hash = CalculateHash(todo);

            return Ok(todo);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo == null) return NotFound();

            _todos.Remove(todo);
            return NoContent();
        }

        [HttpGet("verify")]
        public IActionResult Verify()
        {
            for (int i = 0; i < _todos.Count; i++)
            {
                var current = _todos[i];
                var calculatedHash = CalculateHash(current);
                
                if (current.Hash != calculatedHash)
                    return Conflict(new { message = $"Data Tampered at item {i} (Invalid Hash)" });

                if (i > 0)
                {
                    var previous = _todos[i - 1];
                    if (current.PreviousHash != previous.Hash)
                        return Conflict(new { message = $"Chain Broken at item {i} (PreviousHash Mismatch)" });
                }
            }
            return Ok(new { message = "Chain Valid" });
        }

        private string CalculateHash(Todo todo)
        {
            string rawData = $"{todo.Id}{todo.Title}{todo.Completed}{todo.PreviousHash}";
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes) builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
