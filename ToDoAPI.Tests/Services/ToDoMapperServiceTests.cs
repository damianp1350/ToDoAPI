using ToDoAPI.Models;
using ToDoAPI.Models.Dtos;
using ToDoAPI.Services;

namespace ToDoAPI.Tests.Services;

/// <summary>
///     Contains unit tests for <see cref="ToDoMapperService"/>, validating the mapping functionality 
///     between data transfer objects (DTOs) and <see cref="ToDoModel"/> instances.
/// </summary>
public class ToDoMapperServiceTests
{
    private readonly ToDoMapperService _mapper;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ToDoMapperServiceTests"/> class.
    /// </summary>
    public ToDoMapperServiceTests()
    {
        _mapper = new ToDoMapperService();
    }

    /// <summary>
    ///     Tests that <see cref="ToDoMapperService.MapToModel"/> correctly maps data from a valid 
    ///     <see cref="CreateToDoDto"/> to a new <see cref="ToDoModel"/>.
    /// </summary>
    [Fact]
    public void MapToModel_ValidDto_ReturnsToDoModel()
    {
        var createDto = new CreateToDoDto
        {
            Title = "Test Title",
            Description = "Test Description",
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            PercentComplete = 0
        };

        var result = _mapper.MapToModel(createDto);

        Assert.Equal(createDto.Title, result.Title);
        Assert.Equal(createDto.Description, result.Description);
        Assert.Equal(createDto.ExpiryDate, result.ExpiryDate);
        Assert.Equal(createDto.PercentComplete, result.PercentComplete);
    }

    /// <summary>
    ///     Tests that <see cref="ToDoMapperService.UpdateModel"/> updates an existing <see cref="ToDoModel"/> 
    ///     with data from a valid <see cref="UpdateToDoDto"/>.
    /// </summary>
    [Fact]
    public void UpdateModel_ValidDto_UpdatesToDoModel()
    {
        var updateDto = new UpdateToDoDto
        {
            Id = 1,
            Title = "Updated Title",
            Description = "Updated Description",
            ExpiryDate = DateTime.UtcNow.AddDays(2),
            PercentComplete = 50
        };
        var toDo = new ToDoModel
        {
            Id = 1,
            Title = "Old Title",
            Description = "Old Description",
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            PercentComplete = 0
        };

        _mapper.UpdateModel(updateDto, toDo);

        Assert.Equal(updateDto.Title, toDo.Title);
        Assert.Equal(updateDto.Description, toDo.Description);
        Assert.Equal(updateDto.ExpiryDate, toDo.ExpiryDate);
        Assert.Equal(updateDto.PercentComplete, toDo.PercentComplete);
    }

    /// <summary>
    ///     Tests that <see cref="ToDoMapperService.MapToModel"/> correctly maps a <see cref="CreateToDoDto"/> 
    ///     with a null description, ensuring null values are handled properly.
    /// </summary>
    [Fact]
    public void MapToModel_NullDescription_MapsCorrectly()
    {
        var createDto = new CreateToDoDto
        {
            Title = "Test Title",
            Description = null,
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            PercentComplete = 0
        };

        var result = _mapper.MapToModel(createDto);

        Assert.Equal(createDto.Title, result.Title);
        Assert.Null(result.Description);
        Assert.Equal(createDto.ExpiryDate, result.ExpiryDate);
    }
}
