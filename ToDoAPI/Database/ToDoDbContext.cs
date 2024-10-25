using Microsoft.EntityFrameworkCore;
using ToDoAPI.Models;

namespace ToDoAPI.Database;

/// <summary>
///     Database context for the ToDo API, providing access to the ToDo items stored in the database.
/// </summary>
public class ToDoDbContext : DbContext
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ToDoDbContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">Options for configuring the database context.</param>
    public ToDoDbContext(DbContextOptions<ToDoDbContext> options) : base(options) { }

    /// <summary>
    ///     Gets or sets the <see cref="DbSet{TEntity}"/> for ToDo items.
    /// </summary>
    public DbSet<ToDoModel> ToDos { get; set; }

    /// <summary>
    ///     Configures the model relationships and properties using the <see cref="ModelBuilder"/>.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the model for the database context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
