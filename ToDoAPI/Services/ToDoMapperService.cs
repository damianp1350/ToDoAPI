using ToDoAPI.Models;
using ToDoAPI.Models.Dtos;
using ToDoAPI.Services.IServices;

namespace ToDoAPI.Services;

/// <summary>
///     Provides mapping functionalities to convert data transfer objects (DTOs) into ToDo model instances 
///     and update existing ToDo models from DTOs.
/// </summary>
public class ToDoMapperService : IToDoMapperService
{
    /// <summary>
    ///     Maps the provided <see cref="CreateToDoDto"/> to a new instance of <see cref="ToDoModel"/>.
    /// </summary>
    /// <param name="dto">The data transfer object containing information for the new ToDo item.</param>
    /// <returns>A new <see cref="ToDoModel"/> instance based on the provided DTO.</returns>
    public ToDoModel MapToModel(CreateToDoDto dto) =>
        new ToDoModel
        {
            Title = dto.Title,
            Description = dto.Description,
            ExpiryDate = dto.ExpiryDate,
            PercentComplete = dto.PercentComplete
        };

    /// <summary>
    ///     Updates an existing <see cref="ToDoModel"/> instance with data from the provided <see cref="UpdateToDoDto"/>.
    /// </summary>
    /// <param name="dto">The data transfer object containing updated information for the ToDo item.</param>
    /// <param name="toDo">The existing ToDo model to be updated.</param>
    public void UpdateModel(UpdateToDoDto dto, ToDoModel toDo)
    {
        toDo.Title = dto.Title;
        toDo.Description = dto.Description;
        toDo.ExpiryDate = dto.ExpiryDate;
        toDo.PercentComplete = dto.PercentComplete;
    }
}
