using Microsoft.EntityFrameworkCore;
using ToDoWebApp.Features.ToDo;

namespace ToDoWebApp.Data
{
    public class ToDoDbContext: DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>().HasData(
                new UserModel { Id = 1, Name = "Alice Vance" },
                new UserModel { Id = 2, Name = "Bob Smith" },
                new UserModel { Id = 3, Name = "Charlie Johnson" }
            );
        }

        public DbSet<ToDoModel> ToDoItems { get; set; }
        public DbSet<UserModel> Users { get; set; }
    }
}
