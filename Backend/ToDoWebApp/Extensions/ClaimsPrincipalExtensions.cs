using System.Security.Claims;

namespace ToDoWebApp.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdStr, out var userId))
            {
                // Because your middleware already guarantees the user is valid before 
                // hitting the endpoint, getting a bad ID here means a severe system anomaly.
                throw new InvalidOperationException("User ID claim is missing or malformed.");
            }

            return userId;
        }
    }
}
