using TodoListApp.WebApp.Interfaces;
using TodoListApp.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var apiUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl");

builder.Services.AddHttpClient<ITodoListWebApiService, TodoListWebApiService>(client =>
{
    client.BaseAddress = new Uri(apiUrl!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<ITaskWebApiService, TaskWebApiService>(client =>
{
    client.BaseAddress = new Uri(apiUrl!);
});

builder.Services.AddHttpClient<ITagWebApiService, TagWebApiService>(client =>
{
    client.BaseAddress = new Uri(apiUrl!);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
