// Features/Users/UserEndpoints.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ToDoWebApp.Data;

namespace ToDoWebApp.Features.ToDo
{

    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            // Public endpoint to bootstrap the frontend user-picker
            app.MapGet("/api/users", GetUsersForDropdown).WithTags("Users");
        }

        private static async Task<IResult> GetUsersForDropdown(ToDoDbContext dbContext)
        {
            var users = await dbContext.Users
                .AsNoTracking()
                .Select(u => new UserDropdownDto(u.Id, u.Name))
                .ToListAsync();

            return TypedResults.Ok(users);
        }
    }
}