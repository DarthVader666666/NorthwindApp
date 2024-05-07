namespace Northwind.Application.Models.Roles
{
    public class UserIndexModel
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public ICollection<string>? RoleNames { get; set; }
    }
}
