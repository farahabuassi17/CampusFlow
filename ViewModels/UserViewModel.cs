namespace CampusFlow.Mvc.ViewModels;

using CampusFlow.Mvc.Models.Enums;
public class UserViewModel
{
    public string Id { get; set; } = string.Empty;

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDelete { get; set; }
    public UserType UserType { get; set; }
}
