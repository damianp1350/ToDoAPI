using Microsoft.EntityFrameworkCore;
using ToDoAPI.Database;
using ToDoAPI.Models;
using ToDoAPI.Repositories;

namespace ToDoAPI.Tests.Repositories;

/// <summary>
///     Contains unit tests for <see cref="ToDoRepository"/>, validating CRUD operations and behavior 
///     when interacting with the in-memory database.
/// </summary>
public class ToDoRepositoryTests
{
    private readonly ToDoDbContext _dbContext;
    private readonly ToDoRepository _repository;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ToDoRepositoryTests"/> class, setting up
    ///     an in-memory database context for repository testing.
    /// </summary>
    public ToDoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ToDoDbContext>()
            .UseInMemoryDatabase(databaseName: "ToDoRepositoryTests")
            .Options;

        _dbContext = new ToDoDbContext(options);
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();

        _repository = new ToDoRepository(_dbContext);
    }

    /// <summary>
    ///     Tests that adding a new ToDo item using <see cref="ToDoRepository.AddAsync"/> successfully 
    ///     stores the item in the database.
    /// </summary>
    [Fact]
    public async Task AddAsync_AddsToDo()
    {
        var toDo = new ToDoModel
        {
            Title = "Test ToDo",
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };

        await _repository.AddAsync(toDo);
        var result = await _repository.GetByIdAsync(toDo.Id);

        Assert.NotNull(result);
        Assert.Equal("Test ToDo", result.Title);
    }

    /// <summary>
    ///     Tests that retrieving all ToDo items using <see cref="ToDoRepository.GetAllAsync"/> returns 
    ///     the correct number of items.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ReturnsAllToDos()
    {
        _dbContext.ToDos.AddRange(new[]
        {
            new ToDoModel { Title = "ToDo 1", ExpiryDate = DateTime.UtcNow.AddDays(1) },
            new ToDoModel { Title = "ToDo 2", ExpiryDate = DateTime.UtcNow.AddDays(2) }
        });
        await _dbContext.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        Assert.Equal(2, result.Count);
    }

    /// <summary>
    ///     Tests that updating an existing ToDo item using <see cref="ToDoRepository.UpdateAsync"/> 
    ///     changes the data in the database as expected.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_UpdatesToDo()
    {
        var toDo = new ToDoModel
        {
            Title = "Original Title",
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };
        await _repository.AddAsync(toDo);

        toDo.Title = "Updated Title";
        await _repository.UpdateAsync(toDo);
        var result = await _repository.GetByIdAsync(toDo.Id);

        Assert.NotNull(result);
        Assert.Equal("Updated Title", result!.Title);
    }

    /// <summary>
    ///     Tests that deleting a ToDo item using <see cref="ToDoRepository.DeleteAsync"/> removes 
    ///     the item from the database.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_DeletesToDo()
    {
        var toDo = new ToDoModel
        {
            Title = "ToDelete",
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };
        await _repository.AddAsync(toDo);

        await _repository.DeleteAsync(toDo);
        var result = await _repository.GetByIdAsync(toDo.Id);

        Assert.Null(result);
    }

    /// <summary>
    ///     Tests that retrieving a non-existent ToDo item using <see cref="ToDoRepository.GetByIdAsync"/>
    ///     with a non-existing ID returns null.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(999);
        Assert.Null(result);
    }
}
