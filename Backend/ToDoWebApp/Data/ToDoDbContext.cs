using Microsoft.EntityFrameworkCore;
using ToDoWebApp.Features.ToDo;

namespace ToDoWebApp.Data
{
    public class ToDoDbContext: DbContext
    {
        public ToDoDbContext(DbContextOptions<ToDoDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ToDoModel>()
                .HasIndex(t => new { t.UserId, t.Deleted });

            modelBuilder.Entity<UserModel>().HasData(
                new UserModel { Id = 1, Name = "Alice Vance" },
                new UserModel { Id = 2, Name = "Bob Smith" },
                new UserModel { Id = 3, Name = "Charlie Johnson" }
            );
        }

        public DbSet<ToDoModel> ToDos { get; set; }
        public DbSet<UserModel> Users { get; set; }
    }
}
