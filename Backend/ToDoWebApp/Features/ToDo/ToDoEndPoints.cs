using System.Security.Claims;
using ToDoWebApp.Extensions;

namespace ToDoWebApp.Features.ToDo
{
    // Features/Todos/TodoEndpoints.cs
    public static class TodoEndpoints
    {
        public static void MapTodoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/todos").WithTags("Todos");

            group.MapGet("/", GetAllTodos);
            group.MapGet("/{toDoId}", GetTodo);
            group.MapPost("/", CreateTodo);
            group.MapPut("/", UpdateTodo);
        }

        private static async Task<IResult> GetTodo(
            int toDoId,
            ClaimsPrincipal user,
            IToDoService todoService)
        {
            var userId = user.GetUserId();
            var todo = await todoService.GetByIdForUserAsync(userId, toDoId);

            return TypedResults.Ok(todo);
        }

        private static async Task<IResult> GetAllTodos(
            ClaimsPrincipal user,
            IToDoService todoService)
        {
            var userId = user.GetUserId();
            var todos = await todoService.GetAllForUserAsync(userId);
           
            return TypedResults.Ok(todos);
        }

        private static async Task<IResult> CreateTodo(
            CreateToDoDto dto,
            IToDoService todoService)
        {
            var userId = dto.UserId;
            var result = await todoService.CreateForUserAsync(userId, dto);
            
            return TypedResults.Created($"/api/todos/{result.Id}", result);
        }

        private static async Task<IResult> UpdateTodo(
            UpdateToDoDto dto,
            IToDoService todoService)
        {
            var result = await todoService.UpdateForUserAsync(dto);

            return TypedResults.Ok(result);
        }
    }
}
