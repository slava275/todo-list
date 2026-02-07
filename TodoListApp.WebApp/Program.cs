using TodoListApp.WebApp.Handlers;
using TodoListApp.WebApp.Interfaces;
using TodoListApp.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<JwtHeaderHandler>();

var apiUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl");

builder.Services.AddHttpClient<ITodoListWebApiService, TodoListWebApiService>(client =>
{
    client.BaseAddress = new Uri(apiUrl!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
    .AddHttpMessageHandler<JwtHeaderHandler>();

builder.Services.AddHttpClient<ITaskWebApiService, TaskWebApiService>(client =>
{
    client.BaseAddress = new Uri(apiUrl!);
})
    .AddHttpMessageHandler<JwtHeaderHandler>();

builder.Services.AddHttpClient<ITagWebApiService, TagWebApiService>(client =>
{
    client.BaseAddress = new Uri(apiUrl!);
})
    .AddHttpMessageHandler<JwtHeaderHandler>();

builder.Services.AddHttpClient<ICommentWebApiService, CommentsWebApiService>(client =>
{
    client.BaseAddress = new Uri(apiUrl!);
})
    .AddHttpMessageHandler<JwtHeaderHandler>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(apiUrl!);
});

builder.Services.AddHttpClient<IUserWebApiService, UserWebApiService>(client =>
{
    client.BaseAddress = new Uri(apiUrl!);
})
    .AddHttpMessageHandler<JwtHeaderHandler>();

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
