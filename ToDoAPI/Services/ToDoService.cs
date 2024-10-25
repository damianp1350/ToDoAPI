using ToDoAPI.Models;
using ToDoAPI.Models.Dtos;
using ToDoAPI.Repositories.IRepositories;
using ToDoAPI.Services.IServices;

namespace ToDoAPI.Services;

/// <summary>
///     Provides core business logic for managing ToDo items, handling CRUD operations and custom actions 
///     such as marking completion, setting percentage, and filtering incoming tasks by timeframe.
/// </summary>
public class ToDoService : IToDoService
{
    private readonly IToDoRepository _repository;
    private readonly IToDoMapperService _mapper;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ToDoService"/> class.
    /// </summary>
    /// <param name="repository">Repository to manage data access operations for ToDo items.</param>
    /// <param name="mapper">Service for mapping data transfer objects to model objects.</param>
    public ToDoService(IToDoRepository repository, IToDoMapperService mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>
    ///     Retrieves all ToDo items asynchronously.
    /// </summary>
    /// <returns>A list of all ToDo items.</returns>
    public async Task<List<ToDoModel>> GetAllToDoAsync()
    {
        return await _repository.GetAllAsync();
    }

    /// <summary>
    ///     Retrieves a specific ToDo item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item.</param>
    /// <returns>The ToDo item with the specified ID.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no ToDo item with the given ID is found.</exception>
    public async Task<ToDoModel> GetToDoByIdAsync(int id)
    {
        var toDo = await _repository.GetByIdAsync(id);

        if (toDo == null)
        {
            throw new KeyNotFoundException("ToDo item not found.");
        }

        return toDo;
    }

    /// <summary>
    ///     Retrieves incoming ToDo items based on a specified time frame.
    /// </summary>
    /// <param name="timeFrame">Defines the time range for the incoming ToDo items (Today, NextDay, CurrentWeek).</param>
    /// <returns>A list of ToDo items scheduled within the specified time frame.</returns>
    public async Task<List<ToDoModel>> GetIncomingToDoAsync(IncomingTimeFrame timeFrame)
    {
        DateTime startDate = DateTime.UtcNow.Date;
        DateTime endDate = startDate;

        switch (timeFrame)
        {
            case IncomingTimeFrame.Today:
                endDate = startDate.AddDays(1);
                break;
            case IncomingTimeFrame.NextDay:
                startDate = startDate.AddDays(1);
                endDate = startDate.AddDays(1);
                break;
            case IncomingTimeFrame.CurrentWeek:
                int daysUntilEndOfWeek = DayOfWeek.Sunday - startDate.DayOfWeek;
                startDate = startDate.AddDays(-(int)startDate.DayOfWeek + (int)DayOfWeek.Monday);
                endDate = startDate.AddDays(7);
                break;
        }

        return await _repository.GetIncomingAsync(DateTime.UtcNow, endDate);
    }

    /// <summary>
    ///     Creates a new ToDo item with the specified data.
    /// </summary>
    /// <param name="dto">Data Transfer Object containing information for the new ToDo item.</param>
    /// <returns>The newly created ToDo item.</returns>
    /// <exception cref="ArgumentException">Thrown when required fields in the DTO are invalid or missing.</exception>
    public async Task<ToDoModel> CreateToDoAsync(CreateToDoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            throw new ArgumentException("Title is required.");
        }

        var toDo = _mapper.MapToModel(dto);
        await _repository.AddAsync(toDo);
        return toDo;
    }

    /// <summary>
    ///     Updates an existing ToDo item with new data.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item to update.</param>
    /// <param name="dto">Data Transfer Object containing updated information.</param>
    /// <exception cref="ArgumentException">Thrown when the ID does not match the ToDo ID in the DTO.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the ToDo item to update is not found.</exception>
    public async Task UpdateToDoAsync(int id, UpdateToDoDto dto)
    {
        if (id != dto.Id)
        {
            throw new ArgumentException("The ID provided does not match the ToDo ID.");
        }

        var toDo = await _repository.GetByIdAsync(id);

        if (toDo == null)
        {
            throw new KeyNotFoundException("ToDo item not found.");
        }

        _mapper.UpdateModel(dto, toDo);
        await _repository.UpdateAsync(toDo);
    }

    /// <summary>
    ///     Deletes a ToDo item by id.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item to delete.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the ToDo item to delete is not found.</exception>
    public async Task DeleteToDoAsync(int id)
    {
        var toDo = await GetToDoByIdAsync(id);
        await _repository.DeleteAsync(toDo);
    }

    /// <summary>
    ///     Marks a specific ToDo item as completed.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item to mark as done.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the ToDo item to update is not found.</exception>
    public async Task MarkToDoAsDoneAsync(int id)
    {
        var toDo = await GetToDoByIdAsync(id);
        toDo.IsCompleted = true;
        await _repository.UpdateAsync(toDo);
    }

    /// <summary>
    ///     Sets the completion percentage for a specific ToDo item.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item to update.</param>
    /// <param name="percentComplete">The completion percentage (0-100).</param>
    /// <exception cref="ArgumentException">Thrown when percentComplete is outside the valid range (0-100).</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the ToDo item to update is not found.</exception>
    public async Task SetToDoPercentCompleteAsync(int id, int percentComplete)
    {
        if (percentComplete < 0 || percentComplete > 100)
        {
            throw new ArgumentException("Percent complete must be between 0 and 100.");
        }

        var toDo = await GetToDoByIdAsync(id);
        toDo.PercentComplete = percentComplete;
        await _repository.UpdateAsync(toDo);
    }
}
