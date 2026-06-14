using System.Security.Claims;
using ToDoWebApp.Extensions;

namespace ToDoWebApp.Features.ToDo
{
    public static class TodoEndpoints
    {
        public static void MapTodoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/todos").WithTags("Todos");

            group.MapGet("/", GetAllToDosForUser);
            group.MapGet("/{toDoId}", GetTodo);
            group.MapPost("/", CreateTodo).AddEndpointFilter<ValidationFilter<CreateToDoRequestDto>>();
            group.MapPut("/", UpdateTodo).AddEndpointFilter<ValidationFilter<UpdateToDoRequestDto>>();
            group.MapDelete("/", DeleteTodo);
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

        private static async Task<IResult> GetAllToDosForUser(
            ClaimsPrincipal user,
            IToDoService todoService)
        {
            var userId = user.GetUserId();
            var todos = await todoService.GetToDosForUserAsync(userId);
           
            return TypedResults.Ok(todos);
        }

        private static async Task<IResult> CreateTodo(
            ClaimsPrincipal user,
            CreateToDoRequestDto dto,
            IToDoService todoService)
        {
            var userId = user.GetUserId();
            var result = await todoService.CreateForUserAsync(userId, dto);
            
            return TypedResults.Created($"/api/todos/{result.Id}", result);
        }

        private static async Task<IResult> UpdateTodo(
            ClaimsPrincipal user,
            UpdateToDoRequestDto dto,
            IToDoService todoService)
        {
            var userId = user.GetUserId();
            var result = await todoService.UpdateForUserAsync(userId, dto);

            return TypedResults.Ok(result);
        }

        private static async Task<IResult> DeleteTodo(
            ClaimsPrincipal user,
            int deleteToDoId,
            IToDoService todoService)
        {
            var userId = user.GetUserId();

            await todoService.DeleteForUserAsync(userId, deleteToDoId);
            return TypedResults.Ok();
        }
    }
}
