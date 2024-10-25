using ToDoAPI.Models;

namespace ToDoAPI.Repositories.IRepositories;

/// <summary>
///     Provides data access operations for <see cref="ToDoModel"/> entities, handling CRUD operations 
///     and retrieval based on date ranges.
/// </summary>
public interface IToDoRepository
{
    /// <summary>
    ///     Retrieves all ToDo items from the database asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, containing a list of all <see cref="ToDoModel"/> items.</returns>
    Task<List<ToDoModel>> GetAllAsync();

    /// <summary>
    ///     Retrieves a specific ToDo item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item.</param>
    /// <returns>A task representing the asynchronous operation, containing the <see cref="ToDoModel"/> with the specified ID, or null if not found.</returns>
    Task<ToDoModel?> GetByIdAsync(int id);

    /// <summary>
    ///     Adds a new ToDo item to the database.
    /// </summary>
    /// <param name="toDo">The <see cref="ToDoModel"/> item to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(ToDoModel toDo);

    /// <summary>
    ///     Updates an existing ToDo item in the database.
    /// </summary>
    /// <param name="toDo">The <see cref="ToDoModel"/> item to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(ToDoModel toDo);

    /// <summary>
    ///     Deletes a ToDo item from the database.
    /// </summary>
    /// <param name="toDo">The <see cref="ToDoModel"/> item to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(ToDoModel toDo);

    /// <summary>
    ///     Retrieves ToDo items scheduled within a specified date range.
    /// </summary>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of <see cref="ToDoModel"/> items within the specified date range.</returns>
    Task<List<ToDoModel>> GetIncomingAsync(DateTime startDate, DateTime endDate);
}
