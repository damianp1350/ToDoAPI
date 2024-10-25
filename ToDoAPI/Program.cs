using Microsoft.EntityFrameworkCore;
using ToDoAPI.Database;
using ToDoAPI.Extensions;
using ToDoAPI.Services.IServices;
using ToDoAPI.Services;
using ToDoAPI.Repositories;
using ToDoAPI.Middleware.Filters;
using ToDoAPI.Middleware;
using ToDoAPI.Repositories.IRepositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IToDoService, ToDoService>();
builder.Services.AddScoped<IToDoRepository, ToDoRepository>();
builder.Services.AddScoped<IToDoMapperService, ToDoMapperService>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelAttribute>();
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public partial class Program { }
