using System.Net;
using System.Text.Json;

namespace ToDoAPI.Tests.Integration;

/// <summary>
///     Contains integration tests for the ToDo API endpoints, simulating real HTTP requests to 
///     ensure end-to-end functionality of the API operations.
/// </summary>
public class ToDoApiIntegrationTests : IClassFixture<TestFixture>
{
    private readonly HttpClient _client;
    private readonly TestFixture _fixture;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ToDoApiIntegrationTests"/> class, setting up 
    ///     the HTTP client and fixture for the integration tests.
    /// </summary>
    /// <param name="fixture">The test fixture that provides the HTTP client and factory setup.</param>
    public ToDoApiIntegrationTests(TestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    ///     Tests that the GetAll endpoint returns an OK result with a list of ToDo items.
    /// </summary>
    [Fact]
    public async Task GetAll_ReturnsOkResult_WithData()
    {
        _fixture.Factory.ResetDatabase();

        var response = await _client.GetAsync("/api/todos/get-all");

        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("Test ToDo 1", responseString);
        Assert.Contains("Test ToDo 2", responseString);
    }

    /// <summary>
    ///     Tests that the Create endpoint returns a Created result when a valid ToDo item is posted.
    /// </summary>
    [Fact]
    public async Task Create_ReturnsCreatedResult()
    {
        _fixture.Factory.ResetDatabase();

        var newToDo = new
        {
            Title = "Integration Test ToDo",
            Description = "Description",
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            PercentComplete = 0
        };
        var content = new StringContent(
            JsonSerializer.Serialize(newToDo),
            System.Text.Encoding.UTF8,
            "application/json");

        var response = await _client.PostAsync("/api/todos/create", content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("Integration Test ToDo", responseString);
    }

    /// <summary>
    ///     Tests that the GetIncoming endpoint returns an OK result with a list of incoming ToDo items.
    /// </summary>
    [Fact]
    public async Task GetIncoming_ReturnsOkResult_WithData()
    {
        _fixture.Factory.ResetDatabase();

        var response = await _client.GetAsync("/api/todos/get-incoming?timeFrame=Today");

        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("Test ToDo 1", responseString);
    }

    /// <summary>
    ///     Tests that the Update endpoint returns a NoContent result when a valid update is submitted.
    /// </summary>
    [Fact]
    public async Task Update_ReturnsNoContent()
    {
        _fixture.Factory.ResetDatabase();

        var updateDto = new
        {
            Id = 1,
            Title = "Updated Title",
            Description = "Updated Description",
            ExpiryDate = DateTime.UtcNow.AddDays(2),
            PercentComplete = 50
        };
        var content = new StringContent(
            JsonSerializer.Serialize(updateDto),
            System.Text.Encoding.UTF8,
            "application/json");

        var response = await _client.PutAsync("/api/todos/1/update", content);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    /// <summary>
    ///     Tests that the SetPercentComplete endpoint returns a NoContent result when a valid percent is submitted.
    /// </summary>
    [Fact]
    public async Task SetPercentComplete_ReturnsNoContent()
    {
        _fixture.Factory.ResetDatabase();

        var percentDto = new { PercentComplete = 80 };
        var content = new StringContent(
            JsonSerializer.Serialize(percentDto),
            System.Text.Encoding.UTF8,
            "application/json");

        var response = await _client.PatchAsync("/api/todos/1/percent-complete", content);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    /// <summary>
    ///     Tests that the MarkAsDone endpoint returns a NoContent result when a valid request is sent.
    /// </summary>
    [Fact]
    public async Task MarkAsDone_ReturnsNoContent()
    {
        _fixture.Factory.ResetDatabase();

        var response = await _client.PatchAsync("/api/todos/1/mark-as-done", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    /// <summary>
    ///     Tests that the Delete endpoint returns a NoContent result when an existing item is deleted.
    /// </summary>
    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        _fixture.Factory.ResetDatabase();

        var response = await _client.DeleteAsync("/api/todos/1/delete");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    /// <summary>
    ///     Tests that the Create endpoint returns a BadRequest result when an invalid DTO is posted.
    /// </summary>
    [Fact]
    public async Task Create_InvalidDto_ReturnsBadRequest()
    {
        _fixture.Factory.ResetDatabase();

        var invalidDto = new
        {
            Title = "",
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            PercentComplete = 0
        };

        var content = new StringContent(
            JsonSerializer.Serialize(invalidDto),
            System.Text.Encoding.UTF8,
            "application/json");

        var response = await _client.PostAsync("/api/todos/create", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Title is required.", responseContent);
    }
}
