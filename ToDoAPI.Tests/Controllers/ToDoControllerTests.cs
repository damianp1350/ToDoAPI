using Microsoft.AspNetCore.Mvc;
using ToDoAPI.Controllers;
using ToDoAPI.Models;
using ToDoAPI.Models.Dtos;
using ToDoAPI.Services.IServices;
using Moq;

namespace ToDoAPI.Tests.Controllers;

/// <summary>
///     Contains unit tests for the <see cref="ToDoController"/> class, validating the behavior of each action method 
///     under various scenarios, including success cases and parameter-based variations.
/// </summary>
public class ToDoControllerTests
{
    private readonly ToDoController _controller;
    private readonly Mock<IToDoService> _serviceMock;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ToDoControllerTests"/> class and sets up 
    ///     the mock service dependency.
    /// </summary>
    public ToDoControllerTests()
    {
        _serviceMock = new Mock<IToDoService>();
        _controller = new ToDoController(_serviceMock.Object);
    }

    /// <summary>
    ///     Tests that the GetAll action returns an OK result containing a list of ToDo items.
    /// </summary>
    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfToDos()
    {
        var toDoList = new List<ToDoModel>
        {
            new ToDoModel { Id = 1, Title = "Test 1", ExpiryDate = DateTime.UtcNow },
            new ToDoModel { Id = 2, Title = "Test 2", ExpiryDate = DateTime.UtcNow }
        };
        _serviceMock.Setup(service => service.GetAllToDoAsync()).ReturnsAsync(toDoList);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<ToDoModel>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
    }

    /// <summary>
    ///     Tests that the GetById action returns an OK result with the correct ToDo item when an existing ID is provided.
    /// </summary>
    [Fact]
    public async Task GetById_ExistingId_ReturnsOkResult_WithToDo()
    {
        var toDo = new ToDoModel { Id = 1, Title = "Test", ExpiryDate = DateTime.UtcNow };
        _serviceMock.Setup(service => service.GetToDoByIdAsync(1)).ReturnsAsync(toDo);

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<ToDoModel>(okResult.Value);
        Assert.Equal("Test", returnValue.Title);
    }

    /// <summary>
    ///     Tests that the Create action returns CreatedAtActionResult with the newly created ToDo item.
    /// </summary>
    [Fact]
    public async Task Create_ValidDto_ReturnsCreatedAtAction()
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
        _serviceMock.Setup(service => service.CreateToDoAsync(createDto)).ReturnsAsync(toDoModel);

        var result = await _controller.Create(createDto);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnValue = Assert.IsType<ToDoModel>(createdAtActionResult.Value);
        Assert.Equal("New ToDo", returnValue.Title);
    }

    /// <summary>
    ///     Tests that the GetIncoming action returns an OK result containing a list of incoming ToDo items.
    /// </summary>
    [Fact]
    public async Task GetIncoming_ReturnsOkResult_WithIncomingToDos()
    {
        var timeFrame = IncomingTimeFrame.Today;
        var toDoList = new List<ToDoModel>
        {
            new ToDoModel { Id = 1, Title = "Today ToDo", ExpiryDate = DateTime.UtcNow.AddHours(1) }
        };
        _serviceMock.Setup(service => service.GetIncomingToDoAsync(timeFrame)).ReturnsAsync(toDoList);

        var result = await _controller.GetIncoming(timeFrame);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<ToDoModel>>(okResult.Value);
        Assert.Single(returnValue);
    }

    /// <summary>
    ///     Tests that the Update action with valid data returns a NoContent result.
    /// </summary>
    [Fact]
    public async Task Update_ValidDto_ReturnsNoContent()
    {
        var updateDto = new UpdateToDoDto
        {
            Id = 1,
            Title = "Updated Title",
            Description = "Updated Description",
            ExpiryDate = DateTime.UtcNow.AddDays(2),
            PercentComplete = 50
        };
        _serviceMock.Setup(service => service.UpdateToDoAsync(1, updateDto)).Returns(Task.CompletedTask);

        var result = await _controller.Update(1, updateDto);

        Assert.IsType<NoContentResult>(result);
        _serviceMock.Verify(service => service.UpdateToDoAsync(1, updateDto), Times.Once);
    }

    /// <summary>
    ///     Tests that SetPercentComplete action with a valid percent value returns a NoContent result.
    /// </summary>
    [Fact]
    public async Task SetPercentComplete_ValidPercent_ReturnsNoContent()
    {
        var percentDto = new PercentCompleteDto { PercentComplete = 80 };
        _serviceMock.Setup(service => service.SetToDoPercentCompleteAsync(1, 80)).Returns(Task.CompletedTask);

        var result = await _controller.SetPercentComplete(1, percentDto);

        Assert.IsType<NoContentResult>(result);
        _serviceMock.Verify(service => service.SetToDoPercentCompleteAsync(1, 80), Times.Once);
    }

    /// <summary>
    ///     Tests that the MarkAsDone action with an existing ID returns a NoContent result.
    /// </summary>
    [Fact]
    public async Task MarkAsDone_ExistingId_ReturnsNoContent()
    {
        _serviceMock.Setup(service => service.MarkToDoAsDoneAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.MarkAsDone(1);

        Assert.IsType<NoContentResult>(result);
        _serviceMock.Verify(service => service.MarkToDoAsDoneAsync(1), Times.Once);
    }

    /// <summary>
    ///     Tests that the Delete action with an existing ID returns a NoContent result.
    /// </summary>
    [Fact]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        _serviceMock.Setup(service => service.DeleteToDoAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
        _serviceMock.Verify(service => service.DeleteToDoAsync(1), Times.Once);
    }
}
