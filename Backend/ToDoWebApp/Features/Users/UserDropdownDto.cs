// Features/Users/UserEndpoints.cs
namespace ToDoWebApp.Features.ToDo
{
    internal class UserDropdownDto
    {
        private int id;
        private string name;

        public UserDropdownDto(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}