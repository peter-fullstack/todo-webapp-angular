namespace ToDoWebApp.Features.ToDo
{
    public class ToDoService: IToDoService
    {
        public Task<ToDoDto> GetByIdForUserAsync(int userId, int todoId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ToDoDto>> GetAllForUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<ToDoDto> CreateForUserAsync(int userId, CreateToDoDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ToDoDto> UpdateForUserAsync(UpdateToDoDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ToDoDto> DeleteForUserAsync(int userId, int todoId)
        {
            throw new NotImplementedException();
        }
    }
}