using Microsoft.EntityFrameworkCore;
using ToDoAPI.Database;
using ToDoAPI.Models;
using ToDoAPI.Repositories.IRepositories;

namespace ToDoAPI.Repositories;

/// <summary>
///     Provides data access operations for <see cref="ToDoModel"/> entities, handling CRUD operations 
///     and retrieval based on date ranges.
/// </summary>
public class ToDoRepository : IToDoRepository
{
    private readonly ToDoDbContext _dbContext;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ToDoRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing ToDo data.</param>
    public ToDoRepository(ToDoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    ///     Retrieves all ToDo items from the database asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, containing a list of all <see cref="ToDoModel"/> items.</returns>
    public async Task<List<ToDoModel>> GetAllAsync()
    {
        return await _dbContext.ToDos.OrderBy(toDo => toDo.Id).ToListAsync();
    }

    /// <summary>
    ///     Retrieves a specific ToDo item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item.</param>
    /// <returns>A task representing the asynchronous operation, containing the <see cref="ToDoModel"/> with the specified ID, or null if not found.</returns>
    public async Task<ToDoModel?> GetByIdAsync(int id)
    {
        return await _dbContext.ToDos.FindAsync(id);
    }

    /// <summary>
    ///     Retrieves ToDo items scheduled within a specified date range.
    /// </summary>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of <see cref="ToDoModel"/> items within the specified date range.</returns>
    public async Task<List<ToDoModel>> GetIncomingAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbContext.ToDos
            .Where(t => t.ExpiryDate >= startDate && t.ExpiryDate < endDate)
            .ToListAsync();
    }

    /// <summary>
    ///     Adds a new ToDo item to the database.
    /// </summary>
    /// <param name="toDo">The <see cref="ToDoModel"/> item to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddAsync(ToDoModel toDo)
    {
        _dbContext.ToDos.Add(toDo);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Updates an existing ToDo item in the database.
    /// </summary>
    /// <param name="toDo">The <see cref="ToDoModel"/> item to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateAsync(ToDoModel toDo)
    {
        _dbContext.ToDos.Update(toDo);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Deletes a ToDo item from the database.
    /// </summary>
    /// <param name="toDo">The <see cref="ToDoModel"/> item to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteAsync(ToDoModel toDo)
    {
        _dbContext.ToDos.Remove(toDo);
        await _dbContext.SaveChangesAsync();
    }
}

