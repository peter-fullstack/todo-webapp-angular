namespace ToDoWebApp.Features.ToDo
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ToDoModel> Todos { get; set; } = new();
    }
}
