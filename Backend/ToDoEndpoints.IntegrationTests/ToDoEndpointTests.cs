using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using ToDoWebApp.Data;
using ToDoWebApp.Features.ToDo;

namespace ToDoWebApp.IntegrationTests;

public class TodoApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public TodoApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        // Create a temporary scope to safely resolve the scoped DbContext
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();

            // This sets up the initial users in the in-memory database for testing purposes
            dbContext.Database.EnsureCreated();
        }
    }

    [Fact]
    public async Task GetTodos_WithMissingUserIdHeader_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync($"/api/todos");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorMessage = await response.Content.ReadAsStringAsync();
        errorMessage.Should().Contain("The 'X-User-Id' header is missing or must be an integer.");
    }

    [Fact]
    public async Task GetTodos_ForNonExistantUser_ReturnsUnauthorized()
    {
        // Arrange
        var nonExistantUserId = 35;
       
        // Act
        _client.DefaultRequestHeaders.Add("X-User-Id", nonExistantUserId.ToString());
        var response = await _client.GetAsync($"/api/todos");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTodo_WithValidData_ReturnsCreatedAndSavesToDatabase()
    {
        // Arrange
        var validTodo = new { Title = "Finish Job Application Interview Task" };

        // Act
        _client.DefaultRequestHeaders.Add("X-User-Id", "1");
        var response = await _client.PostAsJsonAsync("/api/todos", validTodo);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTodo = await response.Content.ReadFromJsonAsync<ToDoResponseDto>();
        createdTodo.Should().NotBeNull();
        createdTodo!.Title.Should().Be(validTodo.Title);
    }

    [Fact]
    public async Task UpdateTodo_WithValidData_ReturnsOkAndSavesToDatabase()
    {
        // Arrange
        var todoToUpdate = CreateTodoInDatabase(1, "Finish Job Application Interview Task");

        // Act
        var updateToDoRequest = new UpdateToDoRequestDto
        {
            Id = todoToUpdate.Id,
            Title = "Finish Job Application Interview Task - Nearly Done",
            Description = todoToUpdate.Description,
            IsCompleted = todoToUpdate.IsCompleted,
            UserId = todoToUpdate.UserId
        };

        _client.DefaultRequestHeaders.Add("X-User-Id", "1");
        var response = await _client.PutAsJsonAsync("/api/todos", updateToDoRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var todoResponse = await response.Content.ReadFromJsonAsync<ToDoResponseDto>();
        todoResponse.Should().NotBeNull();
        todoResponse!.Title.Should().Be(updateToDoRequest.Title);
    }

    [Fact]
    public async Task CreateTodo_WithBlankTitle_ReturnsBadRequestAndProblemDetails()
    {
        // Arrange - Testing the FluentValidation logic
        var userId = 1;
        var invalidTodo = new { Title = "" };

        // Act
        _client.DefaultRequestHeaders.Add("X-User-Id", userId.ToString());
        var response = await _client.PostAsJsonAsync("/api/todos", invalidTodo);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().ContainKey("Title");
        problemDetails.Errors["Title"].Should().Contain("Title is required.");
    }

    [Fact]
    public async Task GetTodos_ForUser_ReturnsOkWithListOfTodos()
    {
        // Arrange
        var userId = 2;
        DeleteTodosForUserInDatabase(userId);

        CreateTodoInDatabase(userId, "Test ToDo 1");
        CreateTodoInDatabase(userId, "Test ToDo 2");
        CreateTodoInDatabase(userId, "Test ToDo 3");

        // Act
        _client.DefaultRequestHeaders.Add("X-User-Id", userId.ToString());
        var response = await _client.GetAsync($"/api/todos");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todos = await response.Content.ReadFromJsonAsync<List<ToDoResponseDto>>();
        todos.Should().NotBeNull();
        todos!.Count.Should().Be(3);
    }

    [Fact]
    public async Task GetTodoById_ForUser_ReturnsOkWithTodo()
    {
        // Arrange
        var userId = 2;   
        DeleteTodosForUserInDatabase(userId);

        CreateTodoInDatabase(userId, "Test ToDo 1");
        var todo = CreateTodoInDatabase(userId, "Test ToDo 2");
        CreateTodoInDatabase(userId, "Test ToDo 3");

        // Act
        _client.DefaultRequestHeaders.Add("X-User-Id", userId.ToString());
        var response = await _client.GetAsync($"/api/todos/{todo.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todoResponse = await response.Content.ReadFromJsonAsync<ToDoResponseDto>();
        todoResponse.Should().NotBeNull();
        todoResponse!.Id.Should().Be(todo.Id);
        todoResponse!.Title.Should().Be(todo.Title);
    }


    private ToDoResponseDto CreateTodoInDatabase(int userId, string title)
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
            var todo = new ToDoModel
            {
                Title = title,
                Description = "Test Description",
                UserId = userId,
                IsCompleted = false
            };
            dbContext.ToDos.Add(todo);
            dbContext.SaveChanges();

            return new ToDoResponseDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                UserId = todo.UserId
            };
        }
    }

    private void DeleteTodosForUserInDatabase(int userId)
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
            var todos = dbContext.ToDos.Where(t => t.UserId == userId);
            dbContext.ToDos.RemoveRange(todos);
            dbContext.SaveChanges();
        }
    }
}
