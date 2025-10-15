using Microsoft.OpenApi.Models;
using TodoApi.Models;
using TodoApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",
        policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Todo API",
        Version = "v1",
        Description = "A simple Todo API with filtering and sorting capabilities"
    });

    // Include XML comments if you have them
    try
    {
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    }
    catch
    {

    }
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowBlazor");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();