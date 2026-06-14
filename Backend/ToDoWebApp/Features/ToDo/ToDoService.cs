using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ToDoWebApp.Data;

namespace ToDoWebApp.Features.ToDo
{
    public class ToDoService : IToDoService
    {
        private readonly ToDoDbContext _dbContext;
        private readonly IValidator<CreateToDoRequestDto> _createValidator;
        private readonly IValidator<UpdateToDoRequestDto> _updateValidator;

        public ToDoService(
            ToDoDbContext toDoDbContext, IValidator<CreateToDoRequestDto> createValidator, IValidator<UpdateToDoRequestDto> updateValidator)
        {
            _dbContext = toDoDbContext;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }
        public async Task<ToDoResponseDto> GetByIdForUserAsync(int userId, int todoId)
        {
            var existingToDo = await _dbContext.ToDos.FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId);
            if (existingToDo == null)
            {
                throw new KeyNotFoundException("ToDo item not found for the user.");
            }

            return new ToDoResponseDto
            {
                Id = existingToDo.Id,
                Title = existingToDo.Title,
                Description = existingToDo.Description,
                IsCompleted = existingToDo.IsCompleted,
                UserId = existingToDo.UserId
            };
        }

        public async Task<IEnumerable<ToDoResponseDto>> GetToDosForUserAsync(int userId)
        {
            // TODO: Implement .Skip(page * pageSize).Take(pageSize) for future scaling

            var toDos = await _dbContext
                .ToDos.AsNoTracking()
                .Where(t => t.UserId == userId && !t.Deleted)
                .Select(t => new ToDoResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    UserId = t.UserId
                }).ToListAsync();

            return toDos;
        }

        public async Task<ToDoResponseDto> CreateForUserAsync(int userId, CreateToDoRequestDto dto)
        {
            var validationResult =  await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                // Throwing a standard FluentValidation exception is a best practice.
                throw new FluentValidation.ValidationException(validationResult.Errors);
            }

            var newToDo = new ToDoModel
            {
                Title = dto.Title,
                Description = dto.Description,
                IsCompleted = false,
                UserId = userId
            };

            _dbContext.ToDos.Add(newToDo);
            _dbContext.SaveChanges();

            return new ToDoResponseDto
            {
                Id = newToDo.Id,
                Title = newToDo.Title,
                Description = newToDo.Description,
                IsCompleted = newToDo.IsCompleted,
                UserId = newToDo.UserId
            };
        }

        public async Task<ToDoResponseDto> UpdateForUserAsync(int userId, UpdateToDoRequestDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                 throw new FluentValidation.ValidationException(validationResult.Errors);
            }

            var existingToDo = _dbContext.ToDos.FirstOrDefault(t => t.Id == dto.Id && t.UserId == userId);
            if (existingToDo == null)
            {
                throw new KeyNotFoundException("ToDo item not found for the user.");
            }

            existingToDo.Title = dto.Title;
            existingToDo.Description = dto.Description;
            existingToDo.IsCompleted = dto.IsCompleted;
            existingToDo.UserId = userId;

            _dbContext.SaveChanges();

            return new ToDoResponseDto
            {
                Id = existingToDo.Id,
                Title = existingToDo.Title,
                Description = existingToDo.Description,
                IsCompleted = existingToDo.IsCompleted,
                UserId = existingToDo.UserId
            };
        }

        public async Task DeleteForUserAsync(int userId, int todoId)
        {
            var existingToDo = _dbContext.ToDos.FirstOrDefault(t => t.Id == todoId && t.UserId == userId);
            if (existingToDo == null)
            {
                throw new KeyNotFoundException("ToDo item not found for the user.");
            }

            existingToDo.Deleted = true;
            await _dbContext.SaveChangesAsync();
        }
    }
}