namespace ToDoAPI.Tests.Integration;

/// <summary>
///     Provides a fixture for integration testing, setting up a test server and HTTP client 
///     to simulate requests to the application.
/// </summary>
public class TestFixture : IDisposable
{
    /// <summary>
    ///     Gets the <see cref="WebApplicationFactory{TEntryPoint}"/> used to configure the test server for integration tests.
    /// </summary>
    public WebApplicationFactory<Program> Factory { get; }

    /// <summary>
    ///     Gets the <see cref="HttpClient"/> used to send HTTP requests to the test server.
    /// </summary>
    public HttpClient Client { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TestFixture"/> class, setting up 
    ///     the test server and HTTP client for integration testing.
    /// </summary>
    public TestFixture()
    {
        Factory = new WebApplicationFactory<Program>();
        Client = Factory.CreateClient();
    }

    /// <summary>
    ///     Disposes the HTTP client and test server resources used in integration tests.
    /// </summary>
    public void Dispose()
    {
        Client.Dispose();
        Factory.Dispose();
    }
}
