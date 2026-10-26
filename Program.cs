
using HisaabApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// CORS for Flutter Web during local development
builder.Services.AddCors(options =>
{
    options.AddPolicy("FlutterApp", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// SQL Server database connection
builder.Services.AddDbContext<HisaabDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS must run before controllers
app.UseCors("FlutterApp");

// API health check
app.MapGet("/", () => new
{
    app = "Hisaab API",
    status = "Running"
});

// Database connection test
app.MapGet("/api/health/db", async (HisaabDbContext db) =>
{
    try
    {
        var connected = await db.Database.CanConnectAsync();

        return connected
            ? Results.Ok(new
            {
                status = "Connected",
                database = "HisaabDB"
            })
            : Results.Problem("Database connection failed.");
    }
    catch (Exception ex)
    {
        return Results.Problem(
       title: "Database connection failed",
       detail: ex.Message);
    }
});

app.MapControllers();

app.Run();
