using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ToDoAPI.Database;
using ToDoAPI.Models;

namespace ToDoAPI.Tests.Integration;

/// <summary>
///     Configures a custom web application factory for integration testing, using an in-memory database 
///     to isolate tests and provide seeded test data.
/// </summary>
/// <typeparam name="TStartup">The startup class of the application being tested.</typeparam>
public class WebApplicationFactory<TStartup> : Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<TStartup> where TStartup : class
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    /// <summary>
    ///     Configures the web host to use an in-memory database for testing, replacing the existing 
    ///     database context with a transient in-memory context, and seeds initial data.
    /// </summary>
    /// <param name="builder">The web host builder to configure.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration if present
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ToDoDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Register in-memory database for testing
            services.AddDbContext<ToDoDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            // Build the service provider and seed the database
            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
                db.Database.EnsureCreated();
                SeedDatabase(db);
            }
        });
    }

    /// <summary>
    ///     Resets the database state by deleting and recreating the in-memory database, then re-seeds the initial data.
    /// </summary>
    public void ResetDatabase()
    {
        using (var scope = Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            SeedDatabase(db);
        }
    }

    /// <summary>
    ///     Seeds the in-memory database with initial ToDo items for testing purposes.
    /// </summary>
    /// <param name="db">The <see cref="ToDoDbContext"/> to seed with data.</param>
    private void SeedDatabase(ToDoDbContext db)
    {
        db.ToDos.AddRange(new[]
        {
            new ToDoModel { Id = 1, Title = "Test ToDo 1", ExpiryDate = DateTime.UtcNow.AddHours(1) },
            new ToDoModel { Id = 2, Title = "Test ToDo 2", ExpiryDate = DateTime.UtcNow.AddHours(2) }
        });
        db.SaveChanges();
    }
}
