using ToDoAPI.Models;
using ToDoAPI.Models.Dtos;

namespace ToDoAPI.Services.IServices;

/// <summary>
///     Provides core business logic for managing ToDo items, handling CRUD operations and custom actions 
///     such as marking completion, setting percentage, and filtering incoming tasks by timeframe.
/// </summary>
public interface IToDoService
{
    /// <summary>
    ///     Creates a new ToDo item with the specified data.
    /// </summary>
    /// <param name="dto">Data Transfer Object containing information for the new ToDo item.</param>
    /// <returns>The newly created ToDo item.</returns>
    /// <exception cref="ArgumentException">Thrown when required fields in the DTO are invalid or missing.</exception>
    Task<ToDoModel> CreateToDoAsync(CreateToDoDto dto);

    /// <summary>
    ///     Deletes a ToDo item by id.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item to delete.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the ToDo item to delete is not found.</exception>
    Task DeleteToDoAsync(int id);

    /// <summary>
    ///     Retrieves all ToDo items asynchronously.
    /// </summary>
    /// <returns>A list of all ToDo items.</returns>
    Task<List<ToDoModel>> GetAllToDoAsync();

    /// <summary>
    ///     Retrieves incoming ToDo items based on a specified time frame.
    /// </summary>
    /// <param name="timeFrame">Defines the time range for the incoming ToDo items (Today, NextDay, CurrentWeek).</param>
    /// <returns>A list of ToDo items scheduled within the specified time frame.</returns>
    Task<List<ToDoModel>> GetIncomingToDoAsync(IncomingTimeFrame timeFrame);

    /// <summary>
    ///     Retrieves a specific ToDo item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item.</param>
    /// <returns>The ToDo item with the specified ID.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no ToDo item with the given ID is found.</exception>
    Task<ToDoModel> GetToDoByIdAsync(int id);

    /// <summary>
    ///     Marks a specific ToDo item as completed.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item to mark as done.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the ToDo item to update is not found.</exception>
    Task MarkToDoAsDoneAsync(int id);

    /// <summary>
    ///     Sets the completion percentage for a specific ToDo item.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item to update.</param>
    /// <param name="percentComplete">The completion percentage (0-100).</param>
    /// <exception cref="ArgumentException">Thrown when percentComplete is outside the valid range (0-100).</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the ToDo item to update is not found.</exception>
    Task SetToDoPercentCompleteAsync(int id, int percentComplete);

    /// <summary>
    ///     Updates an existing ToDo item with new data.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item to update.</param>
    /// <param name="dto">Data Transfer Object containing updated information.</param>
    /// <exception cref="ArgumentException">Thrown when the ID does not match the ToDo ID in the DTO.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the ToDo item to update is not found.</exception>
    Task UpdateToDoAsync(int id, UpdateToDoDto dto);
}