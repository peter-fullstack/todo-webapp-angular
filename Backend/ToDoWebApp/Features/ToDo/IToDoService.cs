namespace ToDoWebApp.Features.ToDo
{
    public interface IToDoService
    {
        Task<ToDoResponseDto> GetByIdForUserAsync(int userId, int todoId);
        Task<IEnumerable<ToDoResponseDto>> GetToDosForUserAsync(int userId);
        Task<ToDoResponseDto> CreateForUserAsync(int userId, CreateToDoRequestDto dto);
        Task<ToDoResponseDto> UpdateForUserAsync(int userId, UpdateToDoRequestDto dto);
        Task DeleteForUserAsync(int userId, int todoId);
    }
}
