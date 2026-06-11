namespace ToDoWebApp.Features.ToDo
{
    public interface IToDoService
    {
        Task<ToDoDto> GetByIdForUserAsync(int userId, int todoId);
        Task<IEnumerable<ToDoDto>> GetAllForUserAsync(int userId);
        Task<ToDoDto> CreateForUserAsync(int userId, CreateToDoDto dto);
        Task<ToDoDto> UpdateForUserAsync(UpdateToDoDto dto);
        Task<ToDoDto> DeleteForUserAsync(int userId, int todoId);
    }
}
