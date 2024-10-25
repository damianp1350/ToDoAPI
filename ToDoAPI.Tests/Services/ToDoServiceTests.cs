using ToDoAPI.Models;
using ToDoAPI.Models.Dtos;
using ToDoAPI.Repositories.IRepositories;
using ToDoAPI.Services;
using ToDoAPI.Services.IServices;
using Moq;

namespace ToDoAPI.Tests.Services;

/// <summary>
///     Contains unit tests for <see cref="ToDoService"/>, verifying CRUD and other service 
///     operations on ToDo items.
/// </summary>
public class ToDoServiceTests
{
    private readonly IToDoService _toDoService;
    private readonly Mock<IToDoRepository> _repositoryMock;
    private readonly Mock<IToDoMapperService> _mapperMock;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ToDoServiceTests"/> class, setting up
    ///     mocks for dependencies and instantiating the ToDo service.
    /// </summary>
    public ToDoServiceTests()
    {
        _repositoryMock = new Mock<IToDoRepository>();
        _mapperMock = new Mock<IToDoMapperService>();
        _toDoService = new ToDoService(_repositoryMock.Object, _mapperMock.Object);
    }

    /// <summary>
    ///     Tests that <see cref="ToDoService.GetAllToDoAsync"/> returns a list of ToDo items.
    /// </summary>
    [Fact]
    public async Task GetAllToDoAsync_ReturnsListOfToDos()
    {
        var toDoList = new List<ToDoModel>
        {
            new ToDoModel { Id = 1, Title = "Test 1", ExpiryDate = DateTime.UtcNow },
            new ToDoModel { Id = 2, Title = "Test 2", ExpiryDate = DateTime.UtcNow }
        };
        _repositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(toDoList);

        var result = await _toDoService.GetAllToDoAsync();

        Assert.Equal(2, result.Count);
        _repositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    /// <summary>
    ///     Tests that <see cref="ToDoService.GetToDoByIdAsync"/> retrieves a ToDo item by ID when it exists.
    /// </summary>
    [Fact]
    public async Task GetToDoByIdAsync_ExistingId_ReturnsToDo()
    {
        var toDo = new ToDoModel { Id = 1, Title = "Test", ExpiryDate = DateTime.UtcNow };
        _repositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(toDo);

        var result = await _toDoService.GetToDoByIdAsync(1);

        Assert.Equal("Test", result.Title);
        _repositoryMock.Verify(repo => repo.GetByIdAsync(1), Times.Once);
    }

    /// <summary>
    ///     Tests that <see cref="ToDoService.GetToDoByIdAsync"/> throws a <see cref="KeyNotFoundException"/> 
    ///     when a non-existent ID is provided.
    /// </summary>
    [Fact]
    public async Task GetToDoByIdAsync_NonExistingId_ThrowsException()
    {
        _repositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((ToDoModel?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _toDoService.GetToDoByIdAsync(1));
        _repositoryMock.Verify(repo => repo.GetByIdAsync(1), Times.Once);
    }

    /// <summary>
    ///     Tests that <see cref="ToDoService.CreateToDoAsync"/> creates a new ToDo item and returns it.
    /// </summary>
    [Fact]
    public async Task CreateToDoAsync_ValidDto_ReturnsCreatedToDo()
    {
        var createDto = new CreateToDoDto
        {
            Title = "New ToDo",
            Description = "Description",
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            PercentComplete = 0
        };
        var toDoModel = new ToDoModel
        {
            Id = 1,
            Title = createDto.Title,
            Description = createDto.Description,
            ExpiryDate = createDto.ExpiryDate,
            PercentComplete = createDto.PercentComplete
        };
        _mapperMock.Setup(mapper => mapper.MapToModel(createDto)).Returns(toDoModel);
        _repositoryMock.Setup(repo => repo.AddAsync(toDoModel)).Returns(Task.CompletedTask);

        var result = await _toDoService.CreateToDoAsync(createDto);

        Assert.Equal("New ToDo", result.Title);
        _mapperMock.Verify(mapper => mapper.MapToModel(createDto), Times.Once);
        _repositoryMock.Verify(repo => repo.AddAsync(toDoModel), Times.Once);
    }

    /// <summary>
    ///     Tests that <see cref="ToDoService.GetIncomingToDoAsync"/> retrieves ToDo items scheduled for today.
    /// </summary>
    [Fact]
    public async Task GetIncomingToDoAsync_Today_ReturnsTodayToDos()
    {
        var timeFrame = IncomingTimeFrame.Today;

        var toDoList = new List<ToDoModel>
        {
            new ToDoModel { Id = 1, Title = "Today ToDo", ExpiryDate = DateTime.UtcNow.AddHours(1) },
            new ToDoModel { Id = 2, Title = "Tomorrow ToDo", ExpiryDate = DateTime.UtcNow.AddDays(1).AddHours(1) }
        };

        _repositoryMock.Setup(repo => repo.GetIncomingAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                       .ReturnsAsync(new List<ToDoModel> { toDoList[0] });

        var result = await _toDoService.GetIncomingToDoAsync(timeFrame);

        Assert.Single(result);
        Assert.Equal("Today ToDo", result[0].Title);
        _repositoryMock.Verify(repo => repo.GetIncomingAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
    }

    /// <summary>
    ///     Tests that <see cref="ToDoService.UpdateToDoAsync"/> updates an existing ToDo item when valid data is provided.
    /// </summary>
    [Fact]
    public async Task UpdateToDoAsync_ValidDto_UpdatesToDo()
    {
        var updateDto = new UpdateToDoDto
        {
            Id = 1,
            Title = "Updated Title",
            Description = "Updated Description",
            ExpiryDate = DateTime.UtcNow.AddDays(2),
            PercentComplete = 50
        };

        var existingToDo = new ToDoModel
        {
            Id = 1,
            Title = "Old Title",
            Description = "Old Description",
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            PercentComplete = 20
        };

        _repositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingToDo);
        _mapperMock.Setup(mapper => mapper.UpdateModel(updateDto, existingToDo));
        _repositoryMock.Setup(repo => repo.UpdateAsync(existingToDo)).Returns(Task.CompletedTask);

        await _toDoService.UpdateToDoAsync(1, updateDto);

        _repositoryMock.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        _mapperMock.Verify(mapper => mapper.UpdateModel(updateDto, existingToDo), Times.Once);
        _repositoryMock.Verify(repo => repo.UpdateAsync(existingToDo), Times.Once);
    }

    /// <summary>
    ///     Tests that <see cref="ToDoService.UpdateToDoAsync"/> throws an <see cref="ArgumentException"/> 
    ///     when the provided ID does not match the ToDo ID in the update DTO.
    /// </summary>
    [Fact]
    public async Task UpdateToDoAsync_IdMismatch_ThrowsException()
    {
        var updateDto = new UpdateToDoDto
        {
            Id = 2,
            Title = "Updated Title",
            Description = "Updated Description",
            ExpiryDate = DateTime.UtcNow.AddDays(2),
            PercentComplete = 50
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _toDoService.UpdateToDoAsync(1, updateDto));
    }

    /// <summary>
    ///     Tests that <see cref="ToDoService.DeleteToDoAsync"/> successfully deletes an existing ToDo item
    ///     when a valid ID is provided.
    /// </summary>
    [Fact]
    public async Task DeleteToDoAsync_ExistingId_DeletesToDo()
    {
        var toDo = new ToDoModel { Id = 1, Title = "ToDo to delete", ExpiryDate = DateTime.UtcNow };
        _repositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(toDo);
        _repositoryMock.Setup(repo => repo.DeleteAsync(toDo)).Returns(Task.CompletedTask);

        await _toDoService.DeleteToDoAsync(1);

        _repositoryMock.Verify(repo => repo.DeleteAsync(toDo), Times.Once);
    }

    /// <summary>
    ///     Tests that <see cref="ToDoService.DeleteToDoAsync"/> throws a <see cref="KeyNotFoundException"/> 
    ///     when attempting to delete a ToDo item with a non-existent ID.
    /// </summary>
    [Fact]
    public async Task DeleteToDoAsync_NonExistingId_ThrowsException()
    {
        _repositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((ToDoModel?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _toDoService.DeleteToDoAsync(1));
    }

    /// <summary>
    ///     Tests that <see cref="ToDoService.MarkToDoAsDoneAsync"/> correctly marks a ToDo item 
    ///     as completed when provided with a valid ID.
    /// </summary>
    [Fact]
    public async Task MarkToDoAsDoneAsync_ExistingId_MarksAsDone()
    {
        var toDo = new ToDoModel { Id = 1, Title = "ToDo", IsCompleted = false };
        _repositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(toDo);
        _repositoryMock.Setup(repo => repo.UpdateAsync(toDo)).Returns(Task.CompletedTask);

        await _toDoService.MarkToDoAsDoneAsync(1);

        Assert.True(toDo.IsCompleted);
        _repositoryMock.Verify(repo => repo.UpdateAsync(toDo), Times.Once);
    }

    /// <summary>
    ///     Tests that <see cref="ToDoService.SetToDoPercentCompleteAsync"/> sets the correct percent value for an item.
    /// </summary>
    [Fact]
    public async Task SetToDoPercentCompleteAsync_ValidPercent_SetsPercent()
    {
        var toDo = new ToDoModel { Id = 1, Title = "ToDo", PercentComplete = 20 };
        _repositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(toDo);
        _repositoryMock.Setup(repo => repo.UpdateAsync(toDo)).Returns(Task.CompletedTask);

        await _toDoService.SetToDoPercentCompleteAsync(1, 80);

        Assert.Equal(80, toDo.PercentComplete);
        _repositoryMock.Verify(repo => repo.UpdateAsync(toDo), Times.Once);
    }

    /// <summary>
    ///     Tests that <see cref="ToDoService.SetToDoPercentCompleteAsync"/> throws an <see cref="ArgumentException"/>
    ///     when an invalid percentage is provided (i.e., outside the 0-100 range).
    /// </summary>
    [Fact]
    public async Task SetToDoPercentCompleteAsync_InvalidPercent_ThrowsException()
    {
        var toDo = new ToDoModel { Id = 1, Title = "ToDo", PercentComplete = 20 };
        _repositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(toDo);

        await Assert.ThrowsAsync<ArgumentException>(() => _toDoService.SetToDoPercentCompleteAsync(1, 150));
    }

    /// <summary>
    ///     Tests that <see cref="ToDoService.UpdateToDoAsync"/> throws an <see cref="ArgumentException"/> 
    ///     when the IDs of the provided DTO and the model do not match.
    /// </summary>
    [Fact]
    public async Task UpdateToDoAsync_DtoAndModelIdMismatch_ThrowsArgumentException()
    {
        var updateDto = new UpdateToDoDto
        {
            Id = 2,
            Title = "Updated Title",
            ExpiryDate = DateTime.UtcNow.AddDays(2)
        };

        var existingToDo = new ToDoModel
        {
            Id = 1,
            Title = "Existing ToDo"
        };

        _repositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingToDo);

        await Assert.ThrowsAsync<ArgumentException>(() => _toDoService.UpdateToDoAsync(1, updateDto));
    }

    /// <summary>
    ///     Tests that <see cref="ToDoService.CreateToDoAsync"/> throws an <see cref="ArgumentException"/>
    ///     when attempting to create a ToDo item with invalid data.
    /// </summary>
    [Fact]
    public async Task CreateToDoAsync_InvalidDto_ThrowsValidationException()
    {
        var createDto = new CreateToDoDto
        {
            Title = "",
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            PercentComplete = 0
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _toDoService.CreateToDoAsync(createDto));
    }
}

