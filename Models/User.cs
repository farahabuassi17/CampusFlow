using Microsoft.AspNetCore.Identity;
using CampusFlow.Mvc.Models.Enums;

namespace CampusFlow.Mvc.Models;

public class User : IdentityUser
{
    public bool IsDelete { get; set; }

    public DateTime CreatedAt { get; set; }

    public UserType UserType { get; set; } = UserType.User;
}
