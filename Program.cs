using Microsoft.EntityFrameworkCore;
using BudgetTracker.Endpoints;
using BudgetTracker.Services;
using Scalar.AspNetCore;
using BudgetTracker.Infrastructure;
using BudgetTracker.Infrastructure.Data;

const string DataDir = "data";
const string LogsDir = "logs";

var builder = WebApplication.CreateBuilder(args);

// Dependency Injection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<DbSeeder>();

//builder.Services.AddSingleton(sp => new StorageService(DataDir));
// builder.Services.AddSingleton<ITransactionService, TransactionService>();
// builder.Services.AddSingleton<IUserService, UserService>();
// builder.Services.AddSingleton(sp =>
//    new LoggerService(LogsDir, sp.GetRequiredService<ITransactionService>())
// );

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IUserService, UserService>();
// builder.Services.AddScoped<LoggerService>();


builder.Services.AddOpenApi();

// Error handling / ProblemDetails
builder.Services.AddProblemDetails();

// Swagger (Correct - no NSwag)


var app = builder.Build();

// GLOBAL ERROR HANDLING
app.UseExceptionHandler();
app.UseStatusCodePages();


// --- RUN DB SEEDER ---
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await seeder.SeedAsync();
}

// Swagger + Scalar in dev mode
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Scalar API reference

    app.MapScalarApiReference();

    app.UseSwaggerUi(options =>
     {
         options.DocumentPath = "/openapi/v1.json";
     });

}

// Map your endpoints
app.MapTransactionEndpoints();
app.MapUserEndpoints();

// Test endpoint
app.MapGet("/", () =>
{
    try
    {
        return TypedResults.Ok("Hello World!");
    }
    catch (ArgumentException ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: 400);
    }
})
.Produces<string>(200)
.ProducesProblem(400)
.ProducesProblem(500);

// Run app
app.Run();
