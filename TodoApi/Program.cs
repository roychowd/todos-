using Microsoft.Extensions.FileProviders;
using TodoApi.Repositories;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Your existing services
builder.Services.AddControllers();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",
        policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Enable CORS
app.UseCors("AllowBlazor");

// Serve static files from wwwroot
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot")
    ),
    RequestPath = ""
});

app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles(new StaticFileOptions {ServeUnknownFileTypes = true});

// Fallback for Blazor routing
app.MapFallback(context =>
{
    context.Response.ContentType = "text/html";
    return context.Response.SendFileAsync(Path.Combine(app.Environment.ContentRootPath, "wwwroot", "index.html"));
});

app.Run();
