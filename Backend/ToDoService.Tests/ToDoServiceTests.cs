using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using ToDoWebApp.Data;
using ToDoWebApp.Features.ToDo;

namespace ToDoWebApp.ServiceTests
{
    public class TodoServiceTests
    {
        private ToDoDbContext CreateDbContext()
        {
            // Creates a unique in-memory database name per test to ensure total isolation
            var options = new DbContextOptionsBuilder<ToDoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ToDoDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_ShouldSaveTodoToDatabase_AndReturnMappedDto()
        {
            // Arrange
            var userId = 3;
            var createValidatorMock = new Mock<IValidator<CreateToDoRequestDto>>();
            var updateValidatorMock = new Mock<IValidator<UpdateToDoRequestDto>>();

            using (var dbContext = CreateDbContext())
            {
                var service = new ToDoService(
                    dbContext,
                    createValidatorMock.Object,
                    updateValidatorMock.Object);

                // Act
                var dto = new CreateToDoRequestDto { Title = "Learn .NET Integration Testing" };

                createValidatorMock
                     .Setup(v => v.ValidateAsync(
                         It.IsAny<CreateToDoRequestDto>(),
                         It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new FluentValidation.Results.ValidationResult());

                var result = await service.CreateForUserAsync(userId, dto);

                // Assert the returned object
                result.Should().NotBeNull();
                result.Id.Should().BeGreaterThan(0);
                result.Title.Should().Be(dto.Title);
                result.IsCompleted.Should().BeFalse();

                // Assert the actual database state to ensure side-effects happened
                var dbTodo = await dbContext.ToDos.FindAsync(result.Id);
                dbTodo.Should().NotBeNull();
                dbTodo!.Title.Should().Be(dto.Title);
            }
        }

        [Fact]
        public async Task DeleteAsync_ShouldMarkTodoAsDeleted()
        {
            // Arrange
            var userId = 3;
            var createValidatorMock = new Mock<IValidator<CreateToDoRequestDto>>();
            var updateValidatorMock = new Mock<IValidator<UpdateToDoRequestDto>>();

            using (var dbContext = CreateDbContext())
            {
                var service = new ToDoService(
                    dbContext,
                    createValidatorMock.Object,
                    updateValidatorMock.Object);

                var dtoToDelete = new ToDoModel { Title = "To Be Deleted", UserId = userId, Deleted = false };
                dbContext.ToDos.Add(dtoToDelete);
                await dbContext.SaveChangesAsync();

                // Act
                await service.DeleteForUserAsync(userId, dtoToDelete.Id);

                // Assert the actual database state to ensure side-effects happened
                var dbTodo = await dbContext.ToDos.FindAsync(dtoToDelete.Id);
                dbTodo.Should().NotBeNull();
                dbTodo!.Deleted.Should().BeTrue();
            }
        }
    }
}
