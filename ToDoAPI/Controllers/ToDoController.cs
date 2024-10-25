using Microsoft.AspNetCore.Mvc;
using ToDoAPI.Models;
using ToDoAPI.Models.Dtos;
using ToDoAPI.Services.IServices;

namespace ToDoAPI.Controllers;

/// <summary>
///     API controller for managing ToDo items, providing endpoints for retrieving, creating, updating, 
///     and deleting tasks as well as specific actions like marking completion and setting progress percentage.
/// </summary>
[ApiController]
[Route("api/todos")]
public class ToDoController : ControllerBase
{
    private readonly IToDoService _toDoService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ToDoController"/> class.
    /// </summary>
    /// <param name="toDoService">Service to handle ToDo business logic operations.</param>
    public ToDoController(IToDoService toDoService)
    {
        _toDoService = toDoService;
    }

    /// <summary>
    ///     Retrieves all ToDo items.
    /// </summary>
    /// <returns>A list of all ToDo items with a 200 OK status.</returns>
    [HttpGet("get-all")]
    public async Task<ActionResult<List<ToDoModel>>> GetAll()
    {
        var toDos = await _toDoService.GetAllToDoAsync();
        return Ok(toDos);
    }

    /// <summary>
    ///     Retrieves a specific ToDo item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item.</param>
    /// <returns>The ToDo item with the specified ID and a 200 OK status if found; otherwise, a 404 Not Found status.</returns>
    [HttpGet("{id}/get-by-id")]
    public async Task<ActionResult<ToDoModel>> GetById(int id)
    {
        var toDo = await _toDoService.GetToDoByIdAsync(id);
        return Ok(toDo);
    }

    /// <summary>
    ///     Retrieves incoming ToDo items based on a specified time frame.
    /// </summary>
    /// <param name="timeFrame">Defines the time range for filtering ToDo items (Today, NextDay, CurrentWeek).</param>
    /// <returns>A list of ToDo items within the specified time frame and a 200 OK status.</returns>
    [HttpGet("get-incoming")]
    public async Task<ActionResult<List<ToDoModel>>> GetIncoming([FromQuery] IncomingTimeFrame timeFrame)
    {
        var incomingToDos = await _toDoService.GetIncomingToDoAsync(timeFrame);
        return Ok(incomingToDos);
    }

    /// <summary>
    ///     Creates a new ToDo item.
    /// </summary>
    /// <param name="toDoDto">Data Transfer Object containing information for the new ToDo item.</param>
    /// <returns>The created ToDo item with a 201 Created status and the location of the newly created item.</returns>
    [HttpPost("create")]
    public async Task<ActionResult> Create([FromBody] CreateToDoDto toDoDto)
    {
        var createdToDo = await _toDoService.CreateToDoAsync(toDoDto);
        return CreatedAtAction(nameof(GetById), new { id = createdToDo.Id }, createdToDo);
    }

    /// <summary>
    ///     Updates an existing ToDo item with new data.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item to update.</param>
    /// <param name="toDoDto">Data Transfer Object containing updated information.</param>
    /// <returns>A 204 No Content status if the update is successful.</returns>
    [HttpPut("{id}/update")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateToDoDto toDoDto)
    {
        await _toDoService.UpdateToDoAsync(id, toDoDto);
        return NoContent();
    }

    /// <summary>
    ///     Sets the completion percentage for a specific ToDo item.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item to update.</param>
    /// <param name="dto">Data Transfer Object containing the completion percentage (0-100).</param>
    /// <returns>A 204 No Content status if the update is successful.</returns>
    [HttpPatch("{id}/percent-complete")]
    public async Task<IActionResult> SetPercentComplete(int id, [FromBody] PercentCompleteDto dto)
    {
        await _toDoService.SetToDoPercentCompleteAsync(id, dto.PercentComplete);
        return NoContent();
    }

    /// <summary>
    ///     Marks a specific ToDo item as completed.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item to mark as done.</param>
    /// <returns>A 204 No Content status if the update is successful.</returns>
    [HttpPatch("{id}/mark-as-done")]
    public async Task<IActionResult> MarkAsDone(int id)
    {
        await _toDoService.MarkToDoAsDoneAsync(id);
        return NoContent();
    }

    /// <summary>
    ///     Deletes a ToDo item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item to delete.</param>
    /// <returns>A 204 No Content status if the deletion is successful.</returns>
    [HttpDelete("{id}/delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _toDoService.DeleteToDoAsync(id);
        return NoContent();
    }
}
