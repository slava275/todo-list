using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TodoListApp.Data;
using TodoListApp.Interfaces;
using TodoListApp.Mappings;
using TodoListApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoListDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TodoDbConnectionString")));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<AutomapperProfile>());

builder.Services.AddScoped<ITodoListDatabaseService, TodoListDatabaseService>();
builder.Services.AddScoped<ITaskDatabaseService, TaskDatabaseService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<TodoListDbContext>();

    // Заповнити даними
    DataSeeder.Seed(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
