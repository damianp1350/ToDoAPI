using Microsoft.EntityFrameworkCore;
using ToDoAPI.Database;

namespace ToDoAPI.Extensions;

/// <summary>
///     Provides an extension method for <see cref="IApplicationBuilder"/> to apply pending 
///     migrations or create the database if it does not exist.
/// </summary>
public static class MigrationExtension
{
    /// <summary>
    ///     Applies any pending database migrations if the database is relational. 
    ///     If not relational, ensures the database is created (for testing project).
    /// </summary>
    /// <param name="app">The application builder used to configure the request pipeline.</param>
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using ToDoDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ToDoDbContext>();

        if (dbContext.Database.IsRelational())
        {
            dbContext.Database.Migrate();
        }
        else
        {
            dbContext.Database.EnsureCreated();
        }
    }
}
