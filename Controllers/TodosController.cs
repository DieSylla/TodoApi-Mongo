using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]   // Tous les endpoints nécessitent un token JWT
public class TodosController : ControllerBase
{
    private readonly TodoService _todoService;

    public TodosController(TodoService todoService) =>
        _todoService = todoService;

    // GET : accessible à TOUT utilisateur identifié (même rôle "user")
    [HttpGet]
    public async Task<List<Todo>> Get() =>
        await _todoService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Todo>> Get(string id)
    {
        var todo = await _todoService.GetAsync(id);
        return todo is null ? NotFound() : todo;
    }

    // POST : SEUL rôle "admin"
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Post(Todo newTodo)
    {
        await _todoService.CreateAsync(newTodo);
        return CreatedAtAction(nameof(Get), new { id = newTodo.Id }, newTodo);
    }

    // PUT : SEUL rôle "admin"
    [HttpPut("{id:length(24)}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(string id, Todo updatedTodo)
    {
        var todo = await _todoService.GetAsync(id);
        if (todo is null) return NotFound();

        updatedTodo.Id = todo.Id;
        await _todoService.UpdateAsync(id, updatedTodo);
        return NoContent();
    }

    // DELETE : SEUL rôle "admin"
    [HttpDelete("{id:length(24)}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var todo = await _todoService.GetAsync(id);
        if (todo is null) return NotFound();

        await _todoService.RemoveAsync(id);
        return NoContent();
    }
}