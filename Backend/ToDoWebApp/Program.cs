using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToDoWebApp.Data;
using ToDoWebApp.Features.ToDo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseInMemoryDatabase("TodoDb")); // In-memory Db for simplicity; replace with real DB in production

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddProblemDetails(); // RFC 7807 compliant errors

// Scoped to the request - service uses Entity Framework
builder.Services.AddScoped<IToDoService, ToDoService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware to authenticate user based on a custom header (for demo purposes only - do NOT use in production!)
// This will check and validate the "X-User-Id" header, verify the user exists in the database, and populate HttpContext.User accordingly.
app.Use(async (context, next) =>
{
    // 1. Check if the header is missing or completely un-parseable (Client Error = 400)
    if (!context.Request.Headers.TryGetValue("X-User-Id", out var userIdStr) ||
        !int.TryParse(userIdStr, out var userId))
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync("Bad Request: The 'X-User-Id' header is missing or must be an integer.");
        return;
    }

    // 2. Safely resolve your DbContext within the request's scoped services
    var dbContext = context.RequestServices.GetRequiredService<ToDoDbContext>();
    if (dbContext == null)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync("Internal Server Error: Database context is unavailable.");
        return; // Short-circuit the request pipeline
    }

    // 3. Perform a fast, non-tracking query to verify the user exists
    var userExists = await dbContext.Users
        .AsNoTracking()
        .AnyAsync(u => u.Id == userId);

    if (!userExists)
    {
        // Fail immediately if the client sent an invalid/non-existent user ID
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Unauthorized: Invalid User Identity.");
        return; // Short-circuit the request pipeline
    }

    // 4. User is valid! Populate the ClaimsPrincipal
    var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userIdStr!) };
    var identity = new ClaimsIdentity(claims, "FakeAuth");
    context.User = new ClaimsPrincipal(identity);

    await next(context);
});

app.MapTodoEndpoints();

app.Run();

public partial class Program { }